using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTask3.Models
{
    public partial class Categories
    {
        public Categories()
        {
            ProductCategoryMap = new HashSet<ProductCategoryMap>();
            Products = new HashSet<Products>();
        }
        [Key]
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }       
        public virtual ICollection<ProductCategoryMap> ProductCategoryMap { get; set; }        
        public virtual ICollection<Products> Products { get; set; }
    }
}
