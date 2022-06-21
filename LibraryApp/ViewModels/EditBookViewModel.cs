using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using DbConnect.Models;
using DevExpress.Mvvm;

namespace LibraryApp.ViewModels;

public class EditBookViewModel : BindableBase, ITransient
{
    public EditBookViewModel()
    {
        Task.Run(GetEditBook);
    }

    internal static Book Book = null!;

    private void GetEditBook()
    {
        Id = Book.Id;
        Name = Book.Name;
        Task.Run(() => {
            foreach (var author in DbConnect.Items.Authors.Get())
            {
                SelectedAuthor = Book.Author?.Id ?? 1;
                Application.Current.Dispatcher.Invoke((Action) delegate { Authors.Add($"{author.Surname} {author.Name} {author.Patronymic}"); });
            }
        });
        Task.Run(() => {
            foreach (var style in DbConnect.Items.Styles.Get())
            {
                SelectedStyle = Book.Style?.Id ?? 1;
                Application.Current.Dispatcher.Invoke((Action) delegate { Styles.Add($"{style.Name}"); });
            }
        });
        Task.Run(() => {
            foreach (var category in DbConnect.Items.Categories.Get())
            {
                SelectedCategory = Book.Category?.Id ?? 1;
                Application.Current.Dispatcher.Invoke((Action) delegate { Categories.Add($"{category.Name}"); });
            }
        });
    }
    
    public int Id { get; set; }
    public string? Name { get; set; }
    public ObservableCollection<string> Authors { get; set; } = new();
    public int SelectedAuthor { get; set; }
    public ObservableCollection<string> Styles { get; set; } = new();
    public int SelectedStyle { get; set; }
    public ObservableCollection<string> Categories { get; set; } = new();
    public int SelectedCategory { get; set; }
}