using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using LibraryApp.Services;
using LibraryApp.Views.Pages;

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
        if (DbConnect.DbConnection.IsConnected)
            _pageService.Navigate(new ViewBooksPage());
        else
            MessageBox.Show("Проверьте подключение к базе данных", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
    });
}