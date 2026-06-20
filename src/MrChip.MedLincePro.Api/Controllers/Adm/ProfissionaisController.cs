using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.Controllers;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;

namespace MrChip.MedLincePro.Api.Controllers.Adm;

[Authorize]
[Route("api/v1/adm/profissionais")]
public sealed class ProfissionaisController : ApiControllerBase
{
    private readonly IProfissionalService _service;
    private readonly IUserContext _userContext;

    public ProfissionaisController(IProfissionalService service, IUserContext userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProfissionalDto>>>> Listar([FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.ListarAsync(empresaId, ct));

    [HttpGet("{profissionalId:guid}")]
    public async Task<ActionResult<ApiResponse<ProfissionalDto>>> ObterPorId(Guid profissionalId, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResultOrNotFound(await _service.ObterPorIdAsync(profissionalId, empresaId, ct), "Profissional não encontrado.");

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProfissionalDto>>> Adicionar([FromBody] ProfissionalDto dto, [FromQuery] Guid? bureauId, CancellationToken ct)
    {
        var bureau = bureauId.GetValueOrDefault() == Guid.Empty ? _userContext.BureauId : bureauId!.Value;
        var resultado = await _service.AdicionarAsync(dto, bureau, ct);
        if (!resultado.Sucesso) return BadRequest(ApiResponse<ProfissionalDto>.Falha(resultado.Mensagem));
        return CreatedAtAction(nameof(ObterPorId), new { profissionalId = resultado.Dados!.ProfissionalId }, ApiResponse<ProfissionalDto>.Ok(resultado.Dados, resultado.Mensagem));
    }

    [HttpPut("{profissionalId:guid}")]
    public async Task<ActionResult<ApiResponse>> Atualizar(Guid profissionalId, [FromBody] ProfissionalDto dto, [FromQuery] Guid? bureauId, CancellationToken ct)
    {
        var bureau = bureauId.GetValueOrDefault() == Guid.Empty ? _userContext.BureauId : bureauId!.Value;
        return ToActionResult(await _service.AtualizarAsync(profissionalId, dto, bureau, ct));
    }

    [HttpDelete("{profissionalId:guid}")]
    public async Task<ActionResult<ApiResponse>> Inativar(Guid profissionalId, [FromQuery] Guid? empresaId, CancellationToken ct)
        => ToActionResult(await _service.InativarAsync(profissionalId, empresaId, ct));
}
