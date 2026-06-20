using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Models.Adm;

namespace MrChip.MedLincePro.Business.Interfaces.Adm;

public interface IEmpresaRepository
{
    Task<IEnumerable<Empresa>> ListarAsync(CancellationToken ct = default);
    Task<Empresa?> ObterPorIdAsync(Guid empresaId, CancellationToken ct = default);
    Task<bool> AdicionarAsync(Empresa empresa, CancellationToken ct = default);
    Task<bool> AtualizarAsync(Empresa empresa, CancellationToken ct = default);
    Task<bool> InativarAsync(Guid empresaId, CancellationToken ct = default);
}

public interface IBureauRepository
{
    Task<IEnumerable<Bureau>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<Bureau?> ObterPorIdAsync(Guid bureauId, CancellationToken ct = default);
    Task<bool> AdicionarAsync(Bureau bureau, CancellationToken ct = default);
    Task<bool> AtualizarAsync(Bureau bureau, CancellationToken ct = default);
    Task<bool> InativarAsync(Guid bureauId, Guid empresaId, CancellationToken ct = default);
}

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorLoginParaAutenticacaoAsync(string login, CancellationToken ct = default);
    Task<IEnumerable<UsuarioClaim>> ObterClaimsAsync(Guid usuarioId, CancellationToken ct = default);
    Task<Usuario?> ObterPorIdAsync(Guid usuarioId, Guid bureauId, CancellationToken ct = default);
    Task<List<Usuario>> ListarPorBureauAsync(Guid bureauId, CancellationToken ct = default);
    Task<bool> CriarAsync(UsuarioCreateDto usuario, string passwordHash, CancellationToken ct = default);
    Task<bool> AtualizarAsync(UsuarioUpdateDto usuario, string? novoPasswordHash, CancellationToken ct = default);
    Task<bool> InativarAsync(Guid usuarioId, Guid bureauId, CancellationToken ct = default);
}

public interface IProfissionalRepository
{
    Task<IEnumerable<Profissional>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<Profissional?> ObterPorIdAsync(Guid empresaId, Guid profissionalId, CancellationToken ct = default);
    Task<bool> AdicionarAsync(Profissional profissional, Guid bureauId, CancellationToken ct = default);
    Task<bool> AtualizarAsync(Profissional profissional, Guid bureauId, CancellationToken ct = default);
    Task<bool> InativarAsync(Guid empresaId, Guid profissionalId, CancellationToken ct = default);
}

public interface IUnidadeHospitalarRepository
{
    Task<IEnumerable<UnidadeHospitalar>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<UnidadeHospitalar?> ObterPorCnesAsync(Guid empresaId, string codigoCnes, CancellationToken ct = default);
    Task<bool> AdicionarAsync(Guid empresaId, UnidadeHospitalar unidade, CancellationToken ct = default);
    Task<bool> AtualizarAsync(Guid empresaId, UnidadeHospitalar unidade, CancellationToken ct = default);
    Task<bool> InativarVinculoAsync(Guid empresaId, string codigoCnes, CancellationToken ct = default);
}

public interface IOperadoraRepository
{
    Task<IEnumerable<Operadora>> ListarPorEmpresaAsync(Guid empresaId, bool liberadoParaAgendaMedica = false, bool somenteComWsHabilitado = false, CancellationToken ct = default);
    Task<Operadora?> ObterPorEmpresaAsync(Guid empresaId, Guid operadoraId, CancellationToken ct = default);
    Task<bool> AdicionarAsync(Guid empresaId, Operadora operadora, CancellationToken ct = default);
    Task<bool> AtualizarAsync(Guid empresaId, Operadora operadora, CancellationToken ct = default);
    Task<bool> InativarVinculoAsync(Guid empresaId, Guid operadoraId, CancellationToken ct = default);
}
