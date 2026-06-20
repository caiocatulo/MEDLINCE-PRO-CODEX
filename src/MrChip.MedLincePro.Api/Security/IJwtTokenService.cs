using MrChip.MedLincePro.Business.Dtos.Auth;

namespace MrChip.MedLincePro.Api.Security;

public interface IJwtTokenService
{
    JwtTokenResult GerarToken(AuthContextDto context);
}
