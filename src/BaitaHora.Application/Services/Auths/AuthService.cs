using AutoMapper;
using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Commands.Auth;
using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Application.DTOs.Responses;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IServices.Auths;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Application.IServices.Users;
using BaitaHora.Domain.Commons;
using BaitaHora.Domain.Entities.Users;
using BaitaHora.Domain.Enums;
using BaitaHora.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Services.Auths
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyMemberService _companyMemberService;
        private readonly IPasswordManager _passwordManager;
        private readonly ITokenService _tokenService;
        private readonly IScheduleService _scheduleService;
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ICompanyRepository companyRepository,
            ICompanyMemberService companyMemberService,
            IPasswordManager passwordManager,
            ITokenService tokenService,
            IScheduleService scheduleService,
            ICompanyService companyService,
            IUserService userService,
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _companyMemberService = companyMemberService ?? throw new ArgumentNullException(nameof(companyMemberService));
            _passwordManager = passwordManager ?? throw new ArgumentNullException(nameof(passwordManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserResponse>> RegisterOwnerWithCompanyAsync(RegisterOwnerWithCompanyCommand cmd, CancellationToken ct = default)
        {
            try
            {
                var emailNorm = NormalizeEmail(cmd.User.Email!.Trim());
                var username = NormalizeUsernameRequired(cmd.User.Username!.Trim());

                await EnsureUsernameAvailableAsync(username, ct);
                await EnsureEmailAvailableAsync(emailNorm, ct);

                if (string.IsNullOrWhiteSpace(cmd.Company.Name))
                    return Result<UserResponse>.Failure("Nome da empresa é obrigatório.");

                if (!string.IsNullOrWhiteSpace(cmd.Company.Document) &&
                    await _companyRepository.ExistsByDocumentAsync(cmd.Company.Document, ct))
                    return Result<UserResponse>.Failure("Documento da empresa já está em uso.");

                if (await _companyRepository.ExistsByNameAsync(cmd.Company.Name, ct))
                    return Result<UserResponse>.Failure("Nome da empresa já está em uso.");

                User user = null!;

                await ExecuteInTransactionAsync(async () =>
                {
                    var profile = _mapper.Map<UserProfile>(cmd.User.Profile);

                    user = await _userService.CreateOwnerUserAsync(
                        email: emailNorm,
                        rawPassword: cmd.User.Password,
                        username: username,
                        profile: profile,
                        ct: ct
                    );

                    var company = await _companyService.CreateCompanyAsync(cmd.Company, ct);

                    await _companyMemberService.AddMemberAsync(
                        companyId: company.Id,
                        requesterUserId: user.Id,
                        userId: user.Id,
                        role: CompanyRole.Owner,
                        isActive: true,
                        ct: ct
                    );

                    await _scheduleService.EnsureScheduleAsync(user.Id, company.Id, ct);
                });

                var response = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(response);
            }
            catch (UserException ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao registrar owner {Email}", cmd.User.Email);
                return Result<UserResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar owner {Email}", cmd.User.Email);
                return Result<UserResponse>.Failure("Erro interno ao registrar admin e empresa.");
            }
        }

        public async Task<Result<UserResponse>> RegisterEmployeeAsync(RegisterEmployeeCommand cmd, Guid actorUserId, CancellationToken ct = default)
        {
            try
            {
                var emailNorm = NormalizeEmail(cmd.User.Email!.Trim());
                var username = NormalizeUsernameRequired(cmd.User.Username!.Trim());

                await EnsureUsernameAvailableAsync(username, ct);
                await EnsureEmailAvailableAsync(emailNorm, ct);

                User user = null!;

                await ExecuteInTransactionAsync(async () =>
                {
                    var profile = _mapper.Map<UserProfile>(cmd.User.Profile);

                    user = User.Create(
                        email: emailNorm,
                        rawPassword: cmd.User.Password,
                        profile: profile,
                        username: username,
                        hashFunction: _passwordManager.Hash
                    );

                    await _userRepository.AddAsync(user, ct);

                    await _companyMemberService.AddMemberAsync(
                        companyId: cmd.CompanyId,
                        requesterUserId: actorUserId,
                        userId: user.Id,
                        role: cmd.Role,
                        isActive: true,
                        ct: ct
                    );

                    await _scheduleService.EnsureScheduleAsync(user.Id, cmd.CompanyId, ct);
                });

                var response = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(response);
            }
            catch (UserException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao registrar funcionário {Email}", cmd.User.Email);
                return Result<UserResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar funcionário {Email}", cmd.User.Email);
                return Result<UserResponse>.Failure("Erro interno ao registrar usuário.");
            }
        }

        public async Task<Result<string>> AuthenticateAsync(LoginRequest request, CancellationToken ct)
        {
            try
            {
                var user = IsEmail(request.Identifier)
                    ? await _userRepository.GetByEmailAsync(request.Identifier)
                    : await _userRepository.GetByUsernameAsync(request.Identifier);

                if (user is null || !_passwordManager.Verify(request.Password, user.PasswordHash))
                    return Result<string>.Failure("Usuário ou senha inválidos.");

                var token = _tokenService.GenerateToken(user);
                return Result<string>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao autenticar usuário com identificador {Identifier}", request.Identifier);
                return Result<string>.Failure("Erro interno ao autenticar usuário.");
            }
        }

        private static string NormalizeUsernameRequired(string? usernameRaw)
        {
            if (string.IsNullOrWhiteSpace(usernameRaw))
                throw new UserException("Nome de usuário é obrigatório.");
            return usernameRaw.Trim();
        }

        private static string NormalizeEmail(string emailRaw)
            => emailRaw.Trim().ToLowerInvariant();

        private async Task EnsureUsernameAvailableAsync(string username, CancellationToken ct)
        {
            if (await _userRepository.ExistsByUsernameAsync(username, ct))
                throw new UserException("Nome de usuário já está em uso.");
        }

        private async Task EnsureEmailAvailableAsync(string emailNorm, CancellationToken ct)
        {
            if (await _userRepository.ExistsByEmailAsync(emailNorm, ct))
                throw new UserException("E-mail já está em uso.");
        }

        private async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                await action();
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        private bool IsEmail(string identifier) => identifier.Contains('@');
    }
}