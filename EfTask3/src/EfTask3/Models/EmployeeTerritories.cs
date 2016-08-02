using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfTask3.Models
{
    public partial class EmployeeTerritories
    {
        [ForeignKey("EmployeeId")]
        public long EmployeeId { get; set; }
        [ForeignKey("TerritoryId")]
        public string TerritoryId { get; set; }

        public virtual Employees Employee { get; set; }
        public virtual Territories Territory { get; set; }
    }
}
