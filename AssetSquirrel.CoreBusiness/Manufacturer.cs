using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class Manufacturer
    {
        [Key]
        public int ManufacturerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Equipment>? Equipments { get; set; }
    }
}
