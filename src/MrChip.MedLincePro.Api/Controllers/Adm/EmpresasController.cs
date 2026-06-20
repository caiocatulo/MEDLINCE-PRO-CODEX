using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.Controllers;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces.Adm;

namespace MrChip.MedLincePro.Api.Controllers.Adm;

[Authorize]
[Route("api/v1/adm/empresas")]
public sealed class EmpresasController : ApiControllerBase
{
    private readonly IEmpresaService _service;

    public EmpresasController(IEmpresaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmpresaDto>>>> Listar(CancellationToken ct)
        => ToActionResult(await _service.ListarAsync(ct));

    [HttpGet("{empresaId:guid}")]
    public async Task<ActionResult<ApiResponse<EmpresaDto>>> ObterPorId(Guid empresaId, CancellationToken ct)
        => ToActionResultOrNotFound(await _service.ObterPorIdAsync(empresaId, ct), "Empresa não encontrada.");

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmpresaDto>>> Adicionar([FromBody] EmpresaDto dto, CancellationToken ct)
    {
        var resultado = await _service.AdicionarAsync(dto, ct);
        if (!resultado.Sucesso) return BadRequest(ApiResponse<EmpresaDto>.Falha(resultado.Mensagem));
        return CreatedAtAction(nameof(ObterPorId), new { empresaId = resultado.Dados!.EmpresaId }, ApiResponse<EmpresaDto>.Ok(resultado.Dados, resultado.Mensagem));
    }

    [HttpPut("{empresaId:guid}")]
    public async Task<ActionResult<ApiResponse>> Atualizar(Guid empresaId, [FromBody] EmpresaDto dto, CancellationToken ct)
        => ToActionResult(await _service.AtualizarAsync(empresaId, dto, ct));

    [HttpDelete("{empresaId:guid}")]
    public async Task<ActionResult<ApiResponse>> Inativar(Guid empresaId, CancellationToken ct)
        => ToActionResult(await _service.InativarAsync(empresaId, ct));
}
