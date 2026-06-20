using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.Controllers;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces.Adm;

namespace MrChip.MedLincePro.Api.Controllers.Adm;

[Authorize]
[Route("api/v1/adm/unidades-hospitalares")]
public sealed class UnidadesHospitalaresController : ApiControllerBase
{
    private readonly IUnidadeHospitalarService _service;

    public UnidadesHospitalaresController(IUnidadeHospitalarService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UnidadeHospitalarDto>>>> Listar([FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.ListarAsync(empresaId, ct));

    [HttpGet("{codigoCnes}")]
    public async Task<ActionResult<ApiResponse<UnidadeHospitalarDto>>> ObterPorCnes(string codigoCnes, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResultOrNotFound(await _service.ObterPorCnesAsync(codigoCnes, empresaId, ct), "Unidade hospitalar não encontrada.");

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UnidadeHospitalarDto>>> Adicionar([FromBody] UnidadeHospitalarDto dto, [FromQuery] Guid? empresaId, CancellationToken ct)
    {
        var resultado = await _service.AdicionarAsync(dto, empresaId, ct);
        if (!resultado.Sucesso) return BadRequest(ApiResponse<UnidadeHospitalarDto>.Falha(resultado.Mensagem));
        return CreatedAtAction(nameof(ObterPorCnes), new { codigoCnes = resultado.Dados!.CodigoCnes }, ApiResponse<UnidadeHospitalarDto>.Ok(resultado.Dados, resultado.Mensagem));
    }

    [HttpPut("{codigoCnes}")]
    public async Task<ActionResult<ApiResponse>> Atualizar(string codigoCnes, [FromBody] UnidadeHospitalarDto dto, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.AtualizarAsync(codigoCnes, dto, empresaId, ct));

    [HttpDelete("{codigoCnes}")]
    public async Task<ActionResult<ApiResponse>> Inativar(string codigoCnes, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.InativarAsync(codigoCnes, empresaId, ct));
}
