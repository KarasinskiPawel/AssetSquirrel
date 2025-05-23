using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class Error
    {
        [Key]
        public int IdError { get; set; }
        public DateTime Date { get; set; }
        public string? Service { get; set; }
        public string? Class { get; set; }
        public string? Method { get; set; }
        public string? InnerException { get; set; }
        public string? Message { get; set; }
        public string? Source { get; set; }
        public string? StackTrace { get; set; }
        public string? TargetSite { get; set; }
        public string? UserLogin { get; set; }
    }
}
