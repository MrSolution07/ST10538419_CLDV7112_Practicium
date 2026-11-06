namespace ST10538419_CLDV7112_Practicium.Application.Configuration;

public sealed class FunctionOptions
{
    public const string SectionName = "Functions";

    public string? BaseUrl { get; set; }

    public string? SharedSecret { get; set; }

    public bool Configured =>
        !string.IsNullOrWhiteSpace(BaseUrl) && !string.IsNullOrWhiteSpace(SharedSecret);
}
