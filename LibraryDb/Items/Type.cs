using System;
using System.Collections.Generic;

namespace LibraryDb.Items
{
    public partial class Type
    {
        public Type()
        {
            Readers = new HashSet<Reader>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Reader> Readers { get; set; }
    }
}
