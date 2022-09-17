using System;
using System.Collections.Generic;

namespace LibraryDb.Items
{
    public partial class Loan
    {
        public int Id { get; set; }
        public int? BookId { get; set; }
        public int? ReaderId { get; set; }
        public DateOnly? TakenDate { get; set; }
        public bool? Passed { get; set; }

        public virtual Book? Book { get; set; }
        public virtual Reader? Reader { get; set; }
    }
}
