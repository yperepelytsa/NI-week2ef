﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EfTask3.Models
{
    public partial class Territories
    {
        public Territories()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
        }
        [Key]
        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; set; }
        public long RegionId { get; set; }

        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
        public virtual Region Region { get; set; }
    }
}
