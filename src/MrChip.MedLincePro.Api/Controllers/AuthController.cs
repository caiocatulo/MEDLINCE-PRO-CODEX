using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MrChip.MedLincePro.Api.Security;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Api.ViewModels.Auth;
using MrChip.MedLincePro.Business.Dtos.Auth;
using MrChip.MedLincePro.Business.Interfaces.Auth;

namespace MrChip.MedLincePro.Api.Controllers;

[AllowAnonymous]
[Route("api/v1/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseViewModel>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponseViewModel>>> Login([FromBody] LoginRequestViewModel viewModel, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<LoginResponseViewModel>.Falha("Login e senha são obrigatórios."));

        var resultado = await _authService.AutenticarAsync(new LoginRequestDto
        {
            Login = viewModel.Login,
            Senha = viewModel.Senha
        }, ct);

        if (!resultado.Sucesso || resultado.Dados is null)
            return Unauthorized(ApiResponse<LoginResponseViewModel>.Falha(resultado.Mensagem));

        var token = _jwtTokenService.GerarToken(resultado.Dados);
        var response = new LoginResponseViewModel
        {
            Token = token.Token,
            ExpiraEmUtc = token.ExpiraEmUtc,
            Usuario = resultado.Dados.Usuario,
            Bureau = resultado.Dados.Bureau,
            Empresa = resultado.Dados.Empresa
        };

        return Ok(ApiResponse<LoginResponseViewModel>.Ok(response, "Autenticação realizada com sucesso."));
    }
}
