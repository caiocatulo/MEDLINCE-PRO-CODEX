using MrChip.MedLincePro.Business.Models;

namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class UnidadeHospitalar : Entity
{
    public string CodigoCnes { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string UfCodigo { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string NomeReduzido { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string ContatoEmail { get; set; } = string.Empty;
    public string ContatoNome { get; set; } = string.Empty;
}
