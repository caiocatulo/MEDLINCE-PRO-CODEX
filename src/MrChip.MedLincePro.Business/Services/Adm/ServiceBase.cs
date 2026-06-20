using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Interfaces;

namespace MrChip.MedLincePro.Business.Services.Adm;

public abstract class ServiceBase
{
    private readonly IUserContext _userContext;

    protected ServiceBase(IUserContext userContext)
    {
        _userContext = userContext;
    }

    protected Guid EmpresaIdAutenticada => _userContext.EmpresaId;
    protected Guid BureauIdAutenticado => _userContext.BureauId;
    protected bool UsuarioAutenticado => _userContext.IsAuthenticated;

    protected ResultadoOperacao ValidarAutenticacao()
    {
        return _userContext.IsAuthenticated
            ? ResultadoOperacao.Ok()
            : ResultadoOperacao.Falha("Usuário não autenticado.");
    }

    protected Guid ResolverEmpresaId(Guid? empresaIdInformado)
    {
        var empresaId = empresaIdInformado.GetValueOrDefault();

        return empresaId != Guid.Empty
            ? empresaId
            : _userContext.EmpresaId;
    }

    protected Guid ResolverBureauId(Guid? bureauIdInformado)
    {
        var bureauId = bureauIdInformado.GetValueOrDefault();

        return bureauId != Guid.Empty
            ? bureauId
            : _userContext.BureauId;
    }
}
