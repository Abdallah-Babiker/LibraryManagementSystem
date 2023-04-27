using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public partial class Book
    {
        public int BookId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? Price { get; set; }
        public int? ShelfId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Shelf Shelf { get; set; }
    }
}
