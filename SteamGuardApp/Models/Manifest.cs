using Newtonsoft.Json;
using SteamAuth;

namespace SteamGuardApp.Models;
public class Manifest
{
    [JsonProperty("encrypted")]
    public bool Encrypted { get; set; }

    [JsonProperty("first_run")]
    public bool FirstRun { get; set; } = true;

    [JsonProperty("entries")]
    public List<ManifestEntry> Entries { get; set; } = [];

    [JsonProperty("periodic_checking")]
    public bool PeriodicChecking { get; set; } = false;

    [JsonProperty("periodic_checking_interval")]
    public int PeriodicCheckingInterval { get; set; } = 5;

    [JsonProperty("periodic_checking_checkall")]
    public bool CheckAllAccounts { get; set; } = false;

    [JsonProperty("auto_confirm_market_transactions")]
    public bool AutoConfirmMarketTransactions { get; set; } = false;

    [JsonProperty("auto_confirm_trades")]
    public bool AutoConfirmTrades { get; set; } = false;

    public string ManifestDirectory { get; set; } = string.Empty;

    public Manifest(string manifestDirectory)
    {
        ManifestDirectory = manifestDirectory;
    }

    public void LoadManifest()
    {
        var manifestPath = Path.Combine(ManifestDirectory, "manifest.json");

        try
        {
            var manifestContent = File.ReadAllText(manifestPath);
            JsonConvert.PopulateObject(manifestContent, this);

            var newEntries = new List<ManifestEntry>();

            foreach (var entry in Entries)
            {
                var filename = Path.Combine(ManifestDirectory, entry.Filename);
                if (File.Exists(filename))
                {
                    newEntries.Add(entry);
                }
            }

            Entries = newEntries;
        }
        catch (Exception)
        {
            throw new FileNotFoundException();
        }
    }

    public List<SteamGuardAccount> GetAccounts(string passkey)
    {
        var accounts = new List<SteamGuardAccount>();
        foreach (var entry in Entries)
        {
            var fileText = File.ReadAllText(Path.Combine(ManifestDirectory, entry.Filename));
            if (Encrypted)
            {
                var decryptedText = FileEncryptor.DecryptData(passkey, entry.Salt, entry.IV, fileText);
                if (string.IsNullOrEmpty(decryptedText)) return [];
                fileText = decryptedText;
            }

            var account = JsonConvert.DeserializeObject<SteamGuardAccount>(fileText);
            if (account == null) continue;
            accounts.Add(account);
        }

        return accounts;
    }
}

public class ManifestEntry
{
    [JsonProperty("encryption_iv")]
    public string IV { get; set; }

    [JsonProperty("encryption_salt")]
    public string Salt { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("steamid")]
    public ulong SteamID { get; set; }
}
