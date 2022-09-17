using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using LibraryApp.Services;
using LibraryApp.ViewModels.Edit;
using LibraryApp.Views.Windows;
using LibraryApp.Views.Windows.Edit;
using LibraryDb.Context;
using LibraryDb.Items;
using Microsoft.EntityFrameworkCore;
using Notifications.Wpf.Core;

namespace LibraryApp.ViewModels.PageViews;

public class PageBooksViewModel : BindableBase, IScoped
{
    private PageService _pageService;
    
    public PageBooksViewModel(PageService pageService)
    {
        _pageService = pageService;
        Task.Run(GetItems);
    }

    internal static void GetItems()
    {
        while (true)
        {
            ListBooksCollection = new ObservableCollection<Book>();
            ListBooks = ListBooksCollection;
            try
            {
                using var db = new LibraryDbContext();
                foreach (var book in db.Books
                             .Include(b => b.Author)
                             .Include(b => b.Category)
                             .Include(b => b.Style)
                             .OrderBy(b => b.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate { ListBooksCollection.Add(book); });
                }

                break;
            }
            catch (Exception e)
            {
                new NotificationManager().ShowAsync(new NotificationContent
                {
                    Title = "LibaryAPP",
                    Message = $"{e.Message}",
                    Type = NotificationType.Error
                });
                Application.Current.Dispatcher.Invoke((Action) delegate { new DbSettingWindow().ShowDialog(); });
            }
        }
    }


    public int SelectedIndex { get; set; }

    private string _filterText = null!;

    public string FilterText
    {
        get => _filterText;
        set
        {
            _filterText = value;
            Task.Run(() => FilterTextChanged(_filterText));
            RaisePropertyChanged(nameof(FilterText));
        }
    }
    

    private void FilterTextChanged(string filterText)
    {
        var list = SelectedComboBox switch
        {
            1 => ListBooksCollection.Where(s=>s.Id.ToString().Contains(filterText.ToLower())).ToList(),
            2 => ListBooksCollection.Where(s=>s.Name.ToLower().Contains(filterText.ToLower())).ToList(),
            3 => ListBooksCollection.Where(s=>
                s.Author!.Name.ToLower().Contains(filterText.ToLower())
                || s.Author!.Surname.ToLower().Contains(filterText.ToLower())
                || s.Author!.Patronymic.ToLower().Contains(filterText.ToLower())
            ).ToList(),
            4 => ListBooksCollection.Where(s=>s.Category!.Name.ToLower().Contains(filterText.ToLower())).ToList(),
            5 => ListBooksCollection.Where(s=>s.Style!.Name.ToLower().Contains(filterText.ToLower())).ToList(),
            _ => ListBooksCollection.ToList()
        };
        Application.Current.Dispatcher.Invoke(() => ListBooks = new ObservableCollection<Book>(list));
    }

    private int _selectedComboBox;

    public int SelectedComboBox
    {
        get => _selectedComboBox;
        set
        {
            _selectedComboBox = value;
            Task.Run(() => FilterTextChanged(_filterText));
            RaisePropertyChanged(nameof(SelectedComboBox));
        }
    }

    private static ObservableCollection<Book> ListBooksCollection { get; set; } = new();

    public static ObservableCollection<Book> ListBooks { get; set; } = new();

    public DelegateCommand<int> EditCommand => new(index =>
    {
        var book = ListBooks[index];
        EditBookViewModel.Book = book;
        var editBook = new EditBook();
        editBook.ShowDialog();
        Application.Current.Dispatcher.Invoke((Action) delegate
        {
            using var db = new LibraryDbContext();
            book = db.Books
                .Include(s => s.Author)
                .Include(s => s.Category)
                .Include(s => s.Style)
                .Single(s => s.Id == book.Id);
            ListBooks[SelectedIndex] = book;
            ListBooksCollection = ListBooks;
        });
        if (Application.Current.Windows.OfType<EditBook>().Any())
        {
            Application.Current.Windows.OfType<EditBook>().ForEach(s => s.Close());
        }
    },index=>index != -1);

    public static DelegateCommand<int> DeleteCommand => new(index =>
    {
        if (MessageBox.Show("Вы действительно хотите удалить?", "Удаление", MessageBoxButton.YesNo) ==
            MessageBoxResult.No)
            return;
        
        using var db = new LibraryDbContext();
        var book = ListBooks[index];
        ListBooks.Remove(book);
        db.Books.Remove(book);
        db.SaveChangesAsync();
        new NotificationManager().ShowAsync(new NotificationContent
        {
            Title = "LibaryAPP",
            Message = "Успешно удалено",
            Type = NotificationType.Success
        });
    }, index => index != -1);
}