using System;
using System.Collections.Generic;

namespace LibraryDb.Items
{
    public partial class Reader
    {
        public Reader()
        {
            Loans = new HashSet<Loan>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateOnly? Birthday { get; set; }
        public string? Patronymic { get; set; }
        public int? TypeId { get; set; }

        public virtual Type? Type { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
    }
}
