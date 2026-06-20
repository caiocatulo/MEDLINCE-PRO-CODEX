using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.Controllers;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces.Adm;

namespace MrChip.MedLincePro.Api.Controllers.Adm;

[Authorize]
[Route("api/v1/adm/operadoras")]
public sealed class OperadorasController : ApiControllerBase
{
    private readonly IOperadoraService _service;

    public OperadorasController(IOperadoraService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OperadoraDto>>>> Listar(
        [FromQuery] Guid? empresaId,
        [FromQuery] bool liberadoParaAgendaMedica,
        [FromQuery] bool somenteComWsHabilitado,
        CancellationToken ct)
        => ToActionResult(await _service.ListarAsync(empresaId, liberadoParaAgendaMedica, somenteComWsHabilitado, ct));

    [HttpGet("{operadoraId:guid}")]
    public async Task<ActionResult<ApiResponse<OperadoraDto>>> ObterPorId(Guid operadoraId, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResultOrNotFound(await _service.ObterPorIdAsync(operadoraId, empresaId, ct), "Operadora não encontrada.");

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OperadoraDto>>> Adicionar([FromBody] OperadoraDto dto, [FromQuery] Guid? empresaId, CancellationToken ct)
    {
        var resultado = await _service.AdicionarAsync(dto, empresaId, ct);
        if (!resultado.Sucesso) return BadRequest(ApiResponse<OperadoraDto>.Falha(resultado.Mensagem));
        return CreatedAtAction(nameof(ObterPorId), new { operadoraId = resultado.Dados!.OperadoraId }, ApiResponse<OperadoraDto>.Ok(resultado.Dados, resultado.Mensagem));
    }

    [HttpPut("{operadoraId:guid}")]
    public async Task<ActionResult<ApiResponse>> Atualizar(Guid operadoraId, [FromBody] OperadoraDto dto, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.AtualizarAsync(operadoraId, dto, empresaId, ct));

    [HttpDelete("{operadoraId:guid}")]
    public async Task<ActionResult<ApiResponse>> Inativar(Guid operadoraId, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.InativarAsync(operadoraId, empresaId, ct));
}
