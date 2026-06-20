using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;

namespace MrChip.MedLincePro.Business.Interfaces.Adm;

public interface IEmpresaService
{
    Task<ResultadoOperacao<IEnumerable<EmpresaDto>>> ListarAsync(CancellationToken ct = default);
    Task<ResultadoOperacao<EmpresaDto?>> ObterPorIdAsync(Guid empresaId, CancellationToken ct = default);
    Task<ResultadoOperacao<EmpresaDto>> AdicionarAsync(EmpresaDto dto, CancellationToken ct = default);
    Task<ResultadoOperacao> AtualizarAsync(Guid empresaId, EmpresaDto dto, CancellationToken ct = default);
    Task<ResultadoOperacao> InativarAsync(Guid empresaId, CancellationToken ct = default);
}

public interface IBureauService
{
    Task<ResultadoOperacao<IEnumerable<BureauDto>>> ListarAsync(Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<BureauDto?>> ObterPorIdAsync(Guid bureauId, CancellationToken ct = default);
    Task<ResultadoOperacao<BureauDto>> AdicionarAsync(BureauDto dto, CancellationToken ct = default);
    Task<ResultadoOperacao> AtualizarAsync(Guid bureauId, BureauDto dto, CancellationToken ct = default);
    Task<ResultadoOperacao> InativarAsync(Guid bureauId, CancellationToken ct = default);
}

public interface IUsuarioService
{
    Task<ResultadoOperacao<List<UsuarioDto>>> ListarPorBureauAsync(Guid? bureauId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<UsuarioDto?>> ObterPorIdAsync(Guid usuarioId, Guid? bureauId = null, CancellationToken ct = default);
    Task<ResultadoOperacao> CriarAsync(UsuarioCreateDto dto, CancellationToken ct = default);
    Task<ResultadoOperacao> AtualizarAsync(Guid usuarioId, UsuarioUpdateDto dto, CancellationToken ct = default);
    Task<ResultadoOperacao> InativarAsync(Guid usuarioId, Guid? bureauId = null, CancellationToken ct = default);
}

public interface IProfissionalService
{
    Task<ResultadoOperacao<IEnumerable<ProfissionalDto>>> ListarAsync(Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<ProfissionalDto?>> ObterPorIdAsync(Guid profissionalId, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<ProfissionalDto>> AdicionarAsync(ProfissionalDto dto, Guid bureauId, CancellationToken ct = default);
    Task<ResultadoOperacao> AtualizarAsync(Guid profissionalId, ProfissionalDto dto, Guid bureauId, CancellationToken ct = default);
    Task<ResultadoOperacao> InativarAsync(Guid profissionalId, Guid? empresaId = null, CancellationToken ct = default);
}

public interface IUnidadeHospitalarService
{
    Task<ResultadoOperacao<IEnumerable<UnidadeHospitalarDto>>> ListarAsync(Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<UnidadeHospitalarDto?>> ObterPorCnesAsync(string codigoCnes, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<UnidadeHospitalarDto>> AdicionarAsync(UnidadeHospitalarDto dto, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao> AtualizarAsync(string codigoCnes, UnidadeHospitalarDto dto, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao> InativarAsync(string codigoCnes, Guid? empresaId = null, CancellationToken ct = default);
}

public interface IOperadoraService
{
    Task<ResultadoOperacao<IEnumerable<OperadoraDto>>> ListarAsync(Guid? empresaId = null, bool liberadoParaAgendaMedica = false, bool somenteComWsHabilitado = false, CancellationToken ct = default);
    Task<ResultadoOperacao<OperadoraDto?>> ObterPorIdAsync(Guid operadoraId, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao<OperadoraDto>> AdicionarAsync(OperadoraDto dto, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao> AtualizarAsync(Guid operadoraId, OperadoraDto dto, Guid? empresaId = null, CancellationToken ct = default);
    Task<ResultadoOperacao> InativarAsync(Guid operadoraId, Guid? empresaId = null, CancellationToken ct = default);
}
