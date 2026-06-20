using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.Controllers;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces.Adm;

namespace MrChip.MedLincePro.Api.Controllers.Adm;

[Authorize]
[Route("api/v1/adm/bureaux")]
public sealed class BureauxController : ApiControllerBase
{
    private readonly IBureauService _service;

    public BureauxController(IBureauService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BureauDto>>>> Listar([FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.ListarAsync(empresaId, ct));

    [HttpGet("{bureauId:guid}")]
    public async Task<ActionResult<ApiResponse<BureauDto>>> ObterPorId(Guid bureauId, CancellationToken ct)
        => ToActionResultOrNotFound(await _service.ObterPorIdAsync(bureauId, ct), "Bureau não encontrado.");

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BureauDto>>> Adicionar([FromBody] BureauDto dto, CancellationToken ct)
    {
        var resultado = await _service.AdicionarAsync(dto, ct);
        if (!resultado.Sucesso) return BadRequest(ApiResponse<BureauDto>.Falha(resultado.Mensagem));
        return CreatedAtAction(nameof(ObterPorId), new { bureauId = resultado.Dados!.BureauId }, ApiResponse<BureauDto>.Ok(resultado.Dados, resultado.Mensagem));
    }

    [HttpPut("{bureauId:guid}")]
    public async Task<ActionResult<ApiResponse>> Atualizar(Guid bureauId, [FromBody] BureauDto dto, CancellationToken ct)
        => ToActionResult(await _service.AtualizarAsync(bureauId, dto, ct));

    [HttpDelete("{bureauId:guid}")]
    public async Task<ActionResult<ApiResponse>> Inativar(Guid bureauId, CancellationToken ct)
        => ToActionResult(await _service.InativarAsync(bureauId, ct));
}
