using System.Text.Json.Serialization;

namespace SteamGuardApp.Models;

public class Secrets
{
    [JsonPropertyName("secrets")]
    public List<Secret> AccountSecrets { get; init; } = [];
}

public class Secret
{
    [JsonPropertyName("name")]
    public string AccountName { get; init; }

    [JsonPropertyName("alias")]
    public string? AccountAlias { get; init; }

    [JsonPropertyName("password")]
    public string? Password { get; init; }
}
