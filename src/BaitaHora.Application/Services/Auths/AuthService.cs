using AutoMapper;
using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Application.DTOs.Responses;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IServices.Auths;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Commons;
using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Enums;
using BaitaHora.Domain.Exceptions;
using BaitaHora.Domain.Factories;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Services.Auths
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyMemberRepository _companyMemberRepository;
        private readonly IPasswordManager _passwordManager;
        private readonly ITokenService _tokenService;
        private readonly IAccessService _accessService;
        private readonly IScheduleService _scheduleService;
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ICompanyRepository companyRepository,
            ICompanyMemberRepository companyMemberRepository,
            IPasswordManager passwordManager,
            ITokenService tokenService,
            IAccessService accessService,
            IScheduleService scheduleService,
            ICompanyService companyService,
            IUserService userService,
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _companyMemberRepository = companyMemberRepository ?? throw new ArgumentNullException(nameof(companyMemberRepository));
            _passwordManager = passwordManager ?? throw new ArgumentNullException(nameof(passwordManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _accessService = accessService ?? throw new ArgumentNullException(nameof(accessService));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserResponse>> RegisterOwnerWithCompanyAsync(RegisteOwnerRequest request, CancellationToken ct = default)
        {
            var username = string.IsNullOrWhiteSpace(request.User.Username)
                ? await GenerateUsernameIfEmptyAsync(null)
                : request.User.Username!.Trim();

            if (await _userRepository.ExistsByUsernameAsync(username, ct))
                return Result<UserResponse>.Failure("Nome de usuário já está em uso.");

            if (await _userRepository.ExistsByEmailAsync(request.User.Email, ct))
                return Result<UserResponse>.Failure("E-mail já está em uso.");

            if (string.IsNullOrWhiteSpace(request.Company.Name))
                return Result<UserResponse>.Failure("Nome da empresa é obrigatório.");

            // if (!string.IsNullOrWhiteSpace(request.Company.Document) &&
            //     await _companyRepository.ExistsByDocumentAsync(request.Company.Document, ct))
            //     return Result<UserResponse>.Failure("Documento da empresa já está em uso.");

            if (await _companyRepository.ExistsByNameAsync(request.Company.Name, ct))
                return Result<UserResponse>.Failure("Nome da empresa já está em uso.");

            try
            {
                await _uow.BeginTransactionAsync();

                var userProfile = _mapper.Map<UserProfile>(request.User);
                var user = await _userService.CreateOwnerUserAsync(
                    email: request.User.Email,
                    rawPassword: request.User.Password,
                    username: username,
                    profile: userProfile,
                    ct: ct);

                var company = await _companyService.CreateCompanyAsync(request.Company, ct);

                await _companyService.AddMemberAsync(company.Id, user.Id, CompanyRole.Owner, isActive: true, ct: ct);

                await _scheduleService.EnsureScheduleAsync(user.Id, company.Id, ct);

                await _uow.CommitAsync();

                var response = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(response);
            }
            catch (UserException ex)
            {
                _logger.LogWarning(ex,
                    "Erro de domínio ao bootstrap de empresa/owner {Email}", request.User.Email);
                await _uow.RollbackAsync();
                return Result<UserResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro inesperado ao bootstrap de empresa/owner {Email}", request.User.Email);
                await _uow.RollbackAsync();
                return Result<UserResponse>.Failure("Erro interno ao registrar admin e empresa.");
            }
        }

        public async Task<Result<UserResponse>> RegisterEmployeeAsync(RegisterEmployeeRequest request, Guid actorUserId)
        {
            if (request.CompanyId == Guid.Empty)
                return Result<UserResponse>.Failure("Empresa inválida.");

            if (!await _accessService.CanCreateUsersAsync(actorUserId, request.CompanyId))
                return Result<UserResponse>.Failure("Você não tem permissão para cadastrar usuários nesta empresa.");

            var username = string.IsNullOrWhiteSpace(request.User.Username)
                ? await GenerateUsernameIfEmptyAsync(null)
                : request.User.Username!.Trim();

            if (await _userRepository.ExistsByUsernameAsync(username))
                return Result<UserResponse>.Failure("Nome de usuário já está em uso.");

            if (await _userRepository.ExistsByEmailAsync(request.User.Email))
                return Result<UserResponse>.Failure("E-mail já está em uso.");

            try
            {
                var profile = _mapper.Map<UserProfile>(request.User);

                var user = UserFactory.Create(
                    email: request.User.Email,
                    rawPassword: request.User.Password,
                    profile: profile,
                    username: username,
                    hashFunction: _passwordManager.Hash
                );
                await _userRepository.AddAsync(user);

                var member = new CompanyMember(request.CompanyId, user.Id, request.Role, joinedAt: DateTime.UtcNow, true);
                await _companyMemberRepository.AddAsync(member);

                await _scheduleService.EnsureScheduleAsync(user.Id, request.CompanyId);

                var response = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(response);
            }
            catch (UserException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao registrar usuário {Email}", request.User.Email);
                return Result<UserResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar usuário {Email}", request.User.Email);
                return Result<UserResponse>.Failure("Erro interno ao registrar usuário.");
            }
        }

        public async Task<Result<string>> AuthenticateAsync(LoginRequest request)
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

        private async Task<string> GenerateUsernameIfEmptyAsync(string? username)
        {
            if (!string.IsNullOrWhiteSpace(username))
                return username.Trim();

            int i = 1;
            string generated;
            do
            {
                generated = $"usuario{i++}";
            } while (await _userRepository.ExistsByUsernameAsync(generated));

            return generated;
        }

        private bool IsEmail(string identifier)
        {
            return identifier.Contains('@');
        }
    }
}