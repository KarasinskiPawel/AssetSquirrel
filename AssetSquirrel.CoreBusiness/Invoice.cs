﻿using AssetsSquirrel.CoreBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetSquirrel.CoreBusiness
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Max length 30 chars.")]
        public string? InvoiceNumber { get; set; }
        public string? Description {get;set;}
        public string? FilePath { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? UserId { get; set; } // Foreign key to ApplicationUser

        //Navigation properties
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
        public ICollection<Equipment>? Equipments { get; set; }
    }
}
