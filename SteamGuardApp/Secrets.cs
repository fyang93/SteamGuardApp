using System.Text.Json.Serialization;

namespace SteamGuardApp;

public class Secrets
{
    [JsonPropertyName("secrets")]
    public List<Secret> AccountSecrets { get; init; } = [];
}

public class Secret
{
    [JsonPropertyName("name")]
    public string AccountName { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }
}
