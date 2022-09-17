using System;
using System.Collections.Generic;

namespace LibraryDb.Items
{
    public partial class Book
    {
        public Book()
        {
            Loans = new HashSet<Loan>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public int? StyleId { get; set; }

        public virtual Author? Author { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Style? Style { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}
