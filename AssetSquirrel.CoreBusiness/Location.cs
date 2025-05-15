using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string? Code { get; set; }
        public string? MPK { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
