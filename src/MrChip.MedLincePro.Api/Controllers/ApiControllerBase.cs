using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Common;

namespace MrChip.MedLincePro.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> ToActionResult<T>(ResultadoOperacao<T> resultado)
    {
        if (!resultado.Sucesso)
            return BadRequest(ApiResponse<T>.Falha(resultado.Mensagem));

        return Ok(ApiResponse<T>.Ok(resultado.Dados, resultado.Mensagem));
    }

    protected ActionResult<ApiResponse<T>> ToActionResultOrNotFound<T>(ResultadoOperacao<T?> resultado, string mensagemNaoEncontrado)
        where T : class
    {
        if (!resultado.Sucesso)
            return BadRequest(ApiResponse<T>.Falha(resultado.Mensagem));

        if (resultado.Dados is null)
            return NotFound(ApiResponse<T>.Falha(mensagemNaoEncontrado));

        return Ok(ApiResponse<T>.Ok(resultado.Dados, resultado.Mensagem));
    }

    protected ActionResult<ApiResponse> ToActionResult(ResultadoOperacao resultado)
    {
        if (!resultado.Sucesso)
            return BadRequest(ApiResponse.Falha(resultado.Mensagem));

        return Ok(ApiResponse.Ok(resultado.Mensagem));
    }
}
