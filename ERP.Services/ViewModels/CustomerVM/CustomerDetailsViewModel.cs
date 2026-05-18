using ERP.Services.ViewModels.OrderVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ERP.Services.ViewModels.CustomerVM
{
    public class CustomerDetailsViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$",
       ErrorMessage = "Enter valid Egyptian phone number")]
        public string Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public List<OrderSummaryViewModel> Orders { get; set; } = new();

        public List<PaymentSummaryViewModel> Payments { get; set; } = new();

    }
}
