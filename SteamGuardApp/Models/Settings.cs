using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SteamGuardApp.Models;
public class Settings : INotifyPropertyChanged
{
    private string _username = string.Empty;
    private string _passkey = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
            _username = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 密钥
    /// </summary>
    public string Passkey
    {
        get => _passkey;
        set
        {
            if (value == _passkey) return;
            _passkey = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 将当前实例的属性值更新为另一个实例中的属性值
    /// </summary>
    /// <param name="settings"></param>
    public void UpdateFrom(Settings settings)
    {
        foreach (var property in typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.CanWrite)
            {
                property.SetValue(this, property.GetValue(settings));
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
