using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using EncryptionSample;
using LibraryApp.Views.Windows;
using Microsoft.Win32;

namespace LibraryApp.ViewModels;

public class DbSettingViewModel : BindableBase, IScoped
{
    public DbSettingViewModel()
    {
        var key = Registry.CurrentUser.OpenSubKey("LibraryAPP");
        var host = key.GetValue("host").ToString();
        var user = key.GetValue("user").ToString();
        var pass = key.GetValue("password").ToString();
        var db = key.GetValue("database").ToString();
        var port = key.GetValue("port").ToString();
        Host = host.Decrypt(Program.SecretHash);
        Port = port;
        User = user.Decrypt(Program.SecretHash);
        Password = pass.Decrypt(Program.SecretHash);
        Database = db.Decrypt(Program.SecretHash);
    }
    
    public string Host { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public DelegateCommand ChangeDbStringCommand => new(() =>
    {
        var key = Registry.CurrentUser.CreateSubKey("LibraryAPP");
        key.SetValue("host", Host.Encrypt(Program.SecretHash));
        key.SetValue("user", User.Encrypt(Program.SecretHash));
        key.SetValue("password", Password.Encrypt(Program.SecretHash));
        key.SetValue("database", Database.Encrypt(Program.SecretHash));
        key.SetValue("port", Port);
        Application.Current.Windows.OfType<DbSettingWindow>().Single().Close();
    }, () => !(string.IsNullOrWhiteSpace(Host) 
               || !int.TryParse(Port, out var x)
               || string.IsNullOrWhiteSpace(User) 
               || string.IsNullOrWhiteSpace(Password) 
               || string.IsNullOrWhiteSpace(Database)));

    public static ICommand CancelCommand => new DelegateCommand(() =>
    {
        Application.Current.Windows.OfType<DbSettingWindow>().Single().Close();
    });
}