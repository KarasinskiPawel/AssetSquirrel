using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class HardwareType
    {
        public int HardwareTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Equipment>? Equipments { get; set; }
    }
}
