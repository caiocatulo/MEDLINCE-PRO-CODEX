namespace MrChip.MedLincePro.Data.Connection;

public sealed class DatabaseConnectionOptions
{
    public const string SectionName = "DatabaseConnection";

    public string IpConnections { get; set; } = string.Empty;
    public string SqlUser { get; set; } = string.Empty;
    public string SqlPassword { get; set; } = string.Empty;
}
