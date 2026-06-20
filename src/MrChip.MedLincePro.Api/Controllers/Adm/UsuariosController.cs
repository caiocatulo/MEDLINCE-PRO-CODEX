using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrChip.MedLincePro.Api.Controllers;
using MrChip.MedLincePro.Api.ViewModels;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces.Adm;

namespace MrChip.MedLincePro.Api.Controllers.Adm;

[Authorize]
[Route("api/v1/adm/usuarios")]
public sealed class UsuariosController : ApiControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> Listar([FromQuery] Guid? bureauId, CancellationToken ct)
        => ToActionResult(await _service.ListarPorBureauAsync(bureauId, ct));

    [HttpGet("{usuarioId:guid}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> ObterPorId(Guid usuarioId, [FromQuery] Guid? bureauId, CancellationToken ct)
        => ToActionResultOrNotFound(await _service.ObterPorIdAsync(usuarioId, bureauId, ct), "Usuário não encontrado.");

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Criar([FromBody] UsuarioCreateDto dto, CancellationToken ct)
        => ToActionResult(await _service.CriarAsync(dto, ct));

    [HttpPut("{usuarioId:guid}")]
    public async Task<ActionResult<ApiResponse>> Atualizar(Guid usuarioId, [FromBody] UsuarioUpdateDto dto, CancellationToken ct)
        => ToActionResult(await _service.AtualizarAsync(usuarioId, dto, ct));

    [HttpDelete("{usuarioId:guid}")]
    public async Task<ActionResult<ApiResponse>> Inativar(Guid usuarioId, [FromQuery] Guid? bureauId, CancellationToken ct)
        => ToActionResult(await _service.InativarAsync(usuarioId, bureauId, ct));
}
