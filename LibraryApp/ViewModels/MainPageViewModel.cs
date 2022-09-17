using System.Windows.Input;
using DevExpress.Mvvm;
using LibraryApp.Services;
using LibraryApp.Views.Pages;
using LibraryApp.Views.Windows.Add;

namespace LibraryApp.ViewModels;

public class MainPageViewModel : BindableBase, ISingleton
{
    private readonly PageService _pageService;
    
    public MainPageViewModel(PageService pageService)
    {
        _pageService = pageService;
    }

    public ICommand GoToViewBooksCommand => new DelegateCommand(() =>
    {
        _pageService.Navigate(new ViewBooksPage());
    });

    public static ICommand GoToAddBooksCommand => new DelegateCommand(() =>
    {
        new AddBook().Show();
    });
}