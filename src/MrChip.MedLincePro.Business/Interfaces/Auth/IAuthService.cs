using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Auth;

namespace MrChip.MedLincePro.Business.Interfaces.Auth;

public interface IAuthService
{
    Task<ResultadoOperacao<AuthContextDto>> AutenticarAsync(LoginRequestDto dto, CancellationToken ct = default);
}
