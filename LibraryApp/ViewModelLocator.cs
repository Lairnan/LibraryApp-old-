using LibraryApp.ViewModels;

namespace LibraryApp;

public class ViewModelLocator
{
    // View model main window
    public static MainViewModel MainViewModel => Ioc.Resolve<MainViewModel>();
    
    // View model pages
    public static MainPageViewModel MainPageViewModel => Ioc.Resolve<MainPageViewModel>();
    public static PageBooksViewModel PageBooksViewModel => Ioc.Resolve<PageBooksViewModel>();
    
    // View model edit
    public static EditBookViewModel EditBookViewModel => Ioc.Resolve<EditBookViewModel>();
}