using System.Windows.Input;
using DevExpress.Mvvm;
using LibraryApp.Services;
using LibraryApp.Views.Pages;

namespace LibraryApp.ViewModels;

public class MainPageViewModel : ISingleton
{
    private readonly PageService _pageService;
    
    public MainPageViewModel(PageService pageService)
    {
        _pageService = pageService;
    }

    public ICommand GoToViewBooksCommand => new DelegateCommand(() =>
    {
        _pageService.Navigate(new MainPage());
    });
}