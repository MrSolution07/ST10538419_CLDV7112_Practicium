namespace FinalCloudDev.Application.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "Sqlite";

    public string SqlitePath { get; set; } = "App_Data/local.db";

    public bool UseSqlServer =>
        string.Equals(Provider, "SqlServer", StringComparison.OrdinalIgnoreCase);
}
