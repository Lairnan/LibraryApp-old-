using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DbConnect;
using DbConnect.Items;
using DbConnect.Models;
using DevExpress.Mvvm;
using LibraryApp.Services;
using LibraryApp.Views.Windows.Edit;

namespace LibraryApp.ViewModels;

public class PageBooksViewModel : BindableBase, IScoped
{
    private PageService _pageService;
    
    public PageBooksViewModel(PageService pageService)
    {
        _pageService = pageService;
        Task.Run(GetItems);
    }

    private void GetItems()
    {
        var bookList = new List<Book>();
        foreach (var book in Books.Get())
        {
            bookList.Add(book);
            ListBooksCollection = new ObservableCollection<Book>(bookList);
            Application.Current.Dispatcher.Invoke((Action) delegate
            {
                ListBooks = ListBooksCollection;
            });
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
            0 => ListBooksCollection.Where(s=>s.Id.ToString().Contains(filterText.ToLower())).ToList(),
            1 => ListBooksCollection.Where(s=>s.Name.ToLower().Contains(filterText.ToLower())).ToList(),
            2 => ListBooksCollection.Where(s=>
                s.Author!.Name.ToLower().Contains(filterText.ToLower())
                || s.Author!.Surname.ToLower().Contains(filterText.ToLower())
                || s.Author!.Patronymic.ToLower().Contains(filterText.ToLower())
            ).ToList(),
            3 => ListBooksCollection.Where(s=>s.Category!.Name.ToLower().Contains(filterText.ToLower())).ToList(),
            4 => ListBooksCollection.Where(s=>s.Style!.Name.ToLower().Contains(filterText.ToLower())).ToList(),
            _ => ListBooksCollection.Where(s=>s.Name.ToLower().Contains(filterText.ToLower())).ToList()
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

    private ObservableCollection<Book> ListBooksCollection { get; set; } = new();

    public ObservableCollection<Book> ListBooks { get; set; } = new();

    public DelegateCommand<int> EditCommand => new(index =>
    {
        var book = ListBooks[index];
        EditBookViewModel.Book = book;
        var editBook = new EditBook();
        editBook.ShowDialog();
        EditBookViewModel.ListBooks(ref book);
        Application.Current.Dispatcher.Invoke((Action) delegate
        {
            ListBooks[SelectedIndex] = book;
        });
        if (Application.Current.Windows.Count > 1)
        {
            Application.Current.Windows[1].Close();
        }
    },index=>index != -1);
}