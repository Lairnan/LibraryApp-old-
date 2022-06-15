using LibraryApp.ViewModels;

namespace LibraryApp;

public class ViewModelLocator
{
    public static MainViewModel MainViewModel => Ioc.Resolve<MainViewModel>();
    public static MainPageViewModel MainPageViewModel => Ioc.Resolve<MainPageViewModel>();
    public static PageBooksViewModel PageBooksViewModel => Ioc.Resolve<PageBooksViewModel>();
}