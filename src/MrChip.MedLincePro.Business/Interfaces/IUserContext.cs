namespace MrChip.MedLincePro.Business.Interfaces;

public interface IUserContext
{
    Guid UsuarioId { get; }
    Guid EmpresaId { get; }
    Guid BureauId { get; }
    bool IsAuthenticated { get; }
}
