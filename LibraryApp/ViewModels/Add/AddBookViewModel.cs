using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using LibraryApp.ViewModels.PageViews;
using LibraryApp.Views.Windows.Add;
using LibraryApp.Views.Windows.Edit;
using LibraryDb.Context;
using LibraryDb.Items;
using Notifications.Wpf.Core;
using Style = LibraryDb.Items.Style;

namespace LibraryApp.ViewModels.Add;

public class AddBookViewModel : BindableBase, ITransient
{
    public AddBookViewModel()
    {
        Task.Run(GetEditBook);
    }

    private void GetEditBook()
    {
        using (var db = new LibraryDbContext())
        {
            Id = db.Books.OrderBy(s => s.Id).Last().Id + 1;
        }
        Parallel.Invoke(
            () =>
            {
                using var db = new LibraryDbContext();
                foreach (var author in db.Authors.OrderBy(a => a.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        Authors.Add(author);
                    });
                }
            },
            () =>
            {
                using var db = new LibraryDbContext();
                foreach (var style in db.Styles.OrderBy(s => s.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        Styles.Add($"{style.Name}");
                    });
                }
            },
            () =>
            {
                using var db = new LibraryDbContext();
                foreach (var category in db.Categories.OrderBy(c => c.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        Categories.Add($"{category.Name}");
                    });
                }
            }
        );
    }
    
    public int Id { get; set; }
    public string? Name { get; set; }
    public ObservableCollection<Author> Authors { get; set; } = new();
    public int SelectedAuthor { get; set; }
    public ObservableCollection<string> Styles { get; set; } = new();
    public int SelectedStyle { get; set; }
    public ObservableCollection<string> Categories { get; set; } = new();
    public int SelectedCategory { get; set; }

    public DelegateCommand AddBookCommand => new(() =>
    {
        try
        {
            using var db = new LibraryDbContext();
            db.Books.Add(new Book
            {
                Id = Id,
                Name = Name,
                AuthorId = SelectedAuthor + 1,
                CategoryId = SelectedAuthor + 1,
                StyleId = SelectedStyle + 1,
            });
            
            db.SaveChanges();

            new NotificationManager().ShowAsync(new NotificationContent
            {
                Title = "LibaryAPP",
                Message = "Успешно добавлено",
                Type = NotificationType.Success
            });
            Task.Run(PageBooksViewModel.GetItems);
        }
        catch (Exception ex)
        {
            
            new NotificationManager().ShowAsync(new NotificationContent
            {
                Title = "LibaryAPP",
                Message = ex.Message,
                Type = NotificationType.Error
            });
        }
        finally
        {
            var window = Application.Current.Windows.OfType<AddBook>().Single();
            window.Close();
        }
    }, () => !string.IsNullOrWhiteSpace(Name));
}