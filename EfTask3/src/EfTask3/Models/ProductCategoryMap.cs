using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTask3.Models
{
    public partial class ProductCategoryMap
    {
        [ForeignKey("CategoryId")]
        public long CategoryId { get; set; }
        [ForeignKey("ProductId")]
        public long ProductId { get; set; }
        public virtual Categories Categories { get; set; }
        public virtual Products Products { get; set; }
    }
}
