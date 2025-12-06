namespace SteamGuardApp.Models;

public class UserConfig
{
    public List<UserCredential> Users { get; set; } = new();
}

public class UserCredential
{
    public string Username { get; set; } = string.Empty;
    public string Passkey { get; set; } = string.Empty;
}
