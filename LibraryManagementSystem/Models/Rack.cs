using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public partial class Rack
    {
        public Rack()
        {
            Shelf = new HashSet<Shelf>();
        }

        public int RackId { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Shelf> Shelf { get; set; }
    }
}
