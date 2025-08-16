using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Application.IServices.Auth;
using AutoMapper;
using MediatR;
using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Auth.Requests;

namespace BaitaHora.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ICookieService _cookieService;
        private readonly IMapper _mapper;

        public AuthController(
            ISender mediator,
            ICookieService cookieService,
            IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cookieService = cookieService ?? throw new ArgumentNullException(nameof(cookieService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("register-owner")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerRequest request, CancellationToken ct)
        {
            var command = _mapper.Map<RegisterOwnerWithCompanyCommand>(request);
            var result = await _mediator.Send(command, ct);
            return result.ToActionResult(this);
        }

        [HttpPost("register-employees")]
        [Authorize(Policy = "OwnerOrManagerOfCompany")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var actorUserId))
                return ApiResponseHelper.CreateError("Não autenticado", "Token inválido", 401);

            var cmd = _mapper.Map<RegisterEmployeeCommand>(request) with { ActorUserId = actorUserId };

            var result = await _mediator.Send(cmd, ct);

            return result.ToActionResult(this);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var cmd = new AuthenticateCommand(request.Identifier, request.Password);

            var result = await _mediator.Send(cmd, ct);

            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.Value))
                return result.ToActionResult(this);

            _cookieService.SetJwtCookie(Response, result.Value, TimeSpan.FromHours(1));
            return Ok(ApiResponseHelper.CreateSuccess(
                new { authenticated = true, token = result.Value },
                "Login realizado com sucesso"));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _cookieService.ClearJwtCookie(Response);
            return NoContent();
        }
    }
}