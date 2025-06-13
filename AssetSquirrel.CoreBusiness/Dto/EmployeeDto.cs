using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness.Dto
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public int? EmployeeReferenceNumber { get; set; }
        public string? EmployeeCode { get; set; }
        [Required]
        [MinLength(3)]
        public string? FirstName { get; set; }
        [Required]
        [MinLength(3)]
        public string? LastName { get; set; }
        public string? Email { get; set; }

        [StringLength(9)]
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
