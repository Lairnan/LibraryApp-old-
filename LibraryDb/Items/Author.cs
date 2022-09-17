using System;
using System.Collections.Generic;

namespace LibraryDb.Items
{
    public partial class Author
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
