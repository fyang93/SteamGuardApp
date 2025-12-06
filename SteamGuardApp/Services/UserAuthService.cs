using SteamGuardApp.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public interface IUserAuthService
{
    bool ValidateUser(string username, string passkey);
}

public class UserAuthService : IUserAuthService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UserAuthService> _logger;
    private UserConfig? _userConfig;

    public UserAuthService(IWebHostEnvironment environment, ILogger<UserAuthService> logger)
    {
        _environment = environment;
        _logger = logger;
        LoadUserConfig();
    }

    private void LoadUserConfig()
    {
        try
        {
            var usersYmlPath = Path.Combine(_environment.ContentRootPath, "users.yml");
            if (!File.Exists(usersYmlPath))
            {
                _logger.LogWarning("users.yml 文件不存在于路径: {Path}", usersYmlPath);
                return;
            }

            var yaml = File.ReadAllText(usersYmlPath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            _userConfig = deserializer.Deserialize<UserConfig>(yaml);
            _logger.LogInformation("成功加载 {Count} 个用户配置", _userConfig?.Users?.Count ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载 users.yml 失败");
            _userConfig = null;
        }
    }

    public bool ValidateUser(string username, string passkey)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(passkey))
        {
            return false;
        }

        if (_userConfig == null || !_userConfig.Users.Any())
        {
            _logger.LogWarning("用户配置为空或未加载");
            return false;
        }

        var isValid = _userConfig.Users.Any(u =>
            u.Username == username && u.Passkey == passkey
        );

        if (!isValid)
        {
            _logger.LogWarning("用户验证失败: {Username}", username);
        }

        return isValid;
    }
}