using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DbConnect;
using DevExpress.Mvvm;
using EncryptionSample;
using LibraryApp.Services;
using LibraryApp.Views.Pages;
using LibraryApp.Views.Windows;
using Microsoft.Win32;

namespace LibraryApp.ViewModels;

public class MainViewModel : BindableBase, ISingleton
{
    private readonly PageService _pageService;

    public static string Title => "Главное окно";
    public bool ConnectionStatus { get; private set; }
    public Page CurrentPage { get; private set; }
    
    public MainViewModel(PageService pageService)
    {
        ConnectionStatus = DbConnection.IsConnected;
        _pageService = pageService;
        _pageService.OnPageChanged += page => CurrentPage = page;
        _pageService.Navigate(new MainPage());
        CurrentPage = new MainPage();
    }

    public ICommand GoToBackCommand => new DelegateCommand(() =>
    {
        _pageService.GoToBack();
    },()=>_pageService.CanGoToBack);

    public ICommand ConnectCommand => new DelegateCommand(() =>
    {
        if (ConnectionStatus)
        {
            DbConnection.Close();
            ConnectionStatus = DbConnection.IsConnected;
        }
        else
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        DbConnection.Connect(Program.SecretHash);
                        ConnectionStatus = DbConnection.IsConnected;
                        break;
                    }
                    catch (Exception e)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(e.Message);
                            new DbSettingWindow().ShowDialog();
                        });
                    }
                }
            });
        }
    });

    public static ICommand CloseApplicationCommand => new DelegateCommand(() => Application.Current.Shutdown());

    public static ICommand MinimizeApplicationCommand => new DelegateCommand(() =>
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    });
}