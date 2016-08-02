using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTask3.Models
{
    public partial class OrderDetails
    {
        [ForeignKey("OrderId")]
        public long OrderId { get; set; }
        [ForeignKey("ProductId")]
        public long ProductId { get; set; }
        public string UnitPrice { get; set; }
        public long Quantity { get; set; }
        public string Discount { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
