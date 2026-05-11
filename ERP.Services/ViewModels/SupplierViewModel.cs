using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels
{
    public class SupplierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Total Purchases")]
        public int PurchasesCount { get; set; }

        public IEnumerable<Purchase> Purchases { get; set; } = new List<Purchase>();

    }
}
