using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using LibraryApp.Services;
using LibraryApp.Views.Pages;
using LibraryApp.Views.Windows;
using Notifications.Wpf.Core;
using Notifications.Wpf.Core.Controls;
using Npgsql;

namespace LibraryApp.ViewModels;

public class MainViewModel : BindableBase, ISingleton
{
    private readonly PageService _pageService;
    public static string Title => "Главное окно";
    public bool ConnectionStatus { get; private set; }
    public Page CurrentPage { get; private set; }
    
    public MainViewModel(PageService pageService)
    {
        _pageService = pageService;
        _pageService.OnPageChanged += page => CurrentPage = page;
        _pageService.Navigate(new MainPage());
        CurrentPage = new MainPage();
    }

    public ICommand GoToBackCommand => new DelegateCommand(() =>
    {
        _pageService.GoToBack();
    },()=>_pageService.CanGoToBack);

    public static ICommand CloseApplicationCommand => new DelegateCommand(() => Application.Current.Shutdown());

    public static ICommand MinimizeApplicationCommand => new DelegateCommand(() =>
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;
    });

    public static ICommand ChangeDbSettingCommand => new DelegateCommand(() =>
    {
        new DbSettingWindow().ShowDialog();
    });
}