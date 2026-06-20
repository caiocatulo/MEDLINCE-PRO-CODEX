namespace MrChip.MedLincePro.Business.Interfaces;

public interface IUsuarioPasswordService
{
    string HashParaCompatibilidadeLegada(string senha);
    bool VerificarSenhaLegada(string senhaInformada, string passwordHashPersistido);
}
