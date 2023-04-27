using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public partial class Shelf
    {
        public Shelf()
        {
            Book = new HashSet<Book>();
        }

        public int ShelfId { get; set; }
        public string Code { get; set; }
        public int? RackId { get; set; }

        public virtual Rack Rack { get; set; }
        public virtual ICollection<Book> Book { get; set; }
    }
}
