using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EfTask3.Models
{
    public partial class Shippers
    {
        public Shippers()
        {
            Orders = new HashSet<Orders>();
        }
        [Key]
        public long ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
