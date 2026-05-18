using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels.CustomerVM
{
    public class CustomerViewModel
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Enter valid Egyptian phone number")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(256)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

    }
}
