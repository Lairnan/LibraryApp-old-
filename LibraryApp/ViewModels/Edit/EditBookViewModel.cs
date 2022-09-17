using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using LibraryApp.Views.Windows.Edit;
using LibraryDb.Context;
using LibraryDb.Items;
using Microsoft.EntityFrameworkCore;
using Notifications.Wpf.Core;

namespace LibraryApp.ViewModels.Edit;

public class EditBookViewModel : BindableBase, ITransient
{
    internal static Book Book = null!;
    public EditBookViewModel()
    {
        Task.Run(() => GetEditBook(Book));
    }

    private void GetEditBook(Book book)
    {
        Id = book.Id;
        Name = book.Name;
        Parallel.Invoke(
            () =>
            {
                var i = 0;
                using var db = new LibraryDbContext();
                foreach (var author in db.Authors.OrderBy(a => a.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        if(author.Id == book.AuthorId)
                            SelectedAuthor = i;
                        Authors.Add(author);
                    });
                    i++;
                }
            },
            () =>
            {
                var i = 0;
                using var db = new LibraryDbContext();
                foreach (var style in db.Styles.OrderBy(s => s.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        if(style.Id == book.StyleId)
                            SelectedStyle = i;
                        Styles.Add($"{style.Name}");
                    });
                    i++;
                }
            },
            () =>
            {
                var i = 0;
                using var db = new LibraryDbContext();
                foreach (var category in db.Categories.OrderBy(c => c.Id))
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        if(category.Id == book.CategoryId)
                            SelectedCategory = i;
                        Categories.Add($"{category.Name}");
                    });
                    i++;
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

    public DelegateCommand SaveBookCommand => new(() =>
    {
        try
        {
            using var db = new LibraryDbContext();
            var book = db.Books.Single(s => s.Id == Book.Id);
            book.AuthorId = SelectedAuthor + 1;
            book.CategoryId = SelectedCategory + 1;
            book.StyleId = SelectedStyle + 1;
            book.Name = Name;
            db.SaveChanges();

            Book = book;
            
            new NotificationManager().ShowAsync(new NotificationContent
            {
                Title = "LibaryAPP",
                Message = "Успешно сохранено",
                Type = NotificationType.Success
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            var window = Application.Current.Windows.OfType<EditBook>().SingleOrDefault();
            window?.Close();
        }
    }, () => !string.IsNullOrWhiteSpace(Name));
}