using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using LibraryApp.Services;
using LibraryApp.Views.Pages;

namespace LibraryApp.ViewModels;

public class MainViewModel : BindableBase, ISingleton
{
    private readonly PageService _pageService;

    public static string Title => "Главное окно";
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
}