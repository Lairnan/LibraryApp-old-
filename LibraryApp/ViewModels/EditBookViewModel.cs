using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DbConnect.Items;
using DbConnect.Models;
using DevExpress.Mvvm;

using Style = DbConnect.Models.Style;

namespace LibraryApp.ViewModels;

public class EditBookViewModel : BindableBase, ITransient
{
    public EditBookViewModel()
    {
        Task.Run(GetEditBook);
    }

    internal static Book Book = null!;

    internal static void ListBooks(ref Book book)
    {
        if (_book == null) return;
        book = _book;
        _book = null;
    }

    private static Book? _book;

    private void GetEditBook()
    {
        Id = Book.Id;
        Name = Book.Name;
        Task.Run(() =>
        {
            var i = 0;
            foreach (var author in DbConnect.Items.Authors.Get())
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    if (author.Name == Book.Author?.Name && author.Surname == Book.Author?.Surname && author.Patronymic == Book.Author?.Patronymic)
                        SelectedAuthor = i;
                    Authors.Add(author);
                });
                i++;
            }
        });
        Task.Run(() => {
            var i = 0;
            foreach (var style in DbConnect.Items.Styles.Get())
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    if(style.Name == Book.Style?.Name)
                        SelectedStyle = i;
                    Styles.Add($"{style.Name}");
                });
                i++;
            }
        });
        Task.Run(() => {
            var i = 0;
            foreach (var category in DbConnect.Items.Categories.Get())
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    if(category.Name == Book.Category?.Name)
                        SelectedCategory = i;
                    Categories.Add($"{category.Name}");
                });
                i++;
            }
        });
    }
    
    public int Id { get; set; }
    public string? Name { get; set; }
    public ObservableCollection<Author> Authors { get; set; } = new();
    public int SelectedAuthor { get; set; }
    public ObservableCollection<string> Styles { get; set; } = new();
    public int SelectedStyle { get; set; }
    public ObservableCollection<string> Categories { get; set; } = new();
    public int SelectedCategory { get; set; }

    public DelegateCommand SaveBookCommand => new(() =>
    {
        try
        {
            Books.Update(Id, Name, SelectedAuthor + 1, SelectedCategory + 1, SelectedStyle + 1);
            MessageBox.Show("Успешно сохранено");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            _book = new Book
            {
                Id = Id,
                Name = Name,
                Author = new Author
                {
                    Id = SelectedAuthor + 1,
                    Name = Authors[SelectedAuthor].Name,
                    Surname = Authors[SelectedAuthor].Surname,
                    Patronymic = Authors[SelectedAuthor].Patronymic,
                },
                Category = new Category
                {
                    Id = SelectedCategory + 1,
                    Name = Categories[SelectedCategory].Trim()
                },
                Style = new Style
                {
                    Id = SelectedStyle + 1,
                    Name = Styles[SelectedStyle].Trim()
                },
            };
            var window = Application.Current.Windows[1];
            window.Close();
        }
    }, () => !string.IsNullOrWhiteSpace(Name));
}