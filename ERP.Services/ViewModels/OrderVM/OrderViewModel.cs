using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Domain.Enums;

namespace ERP.Services.ViewModels.OrderVM
{
    public class OrderViewModel
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Paid Amount")]
        [Range(0, double.MaxValue)]
        public decimal PaidAmount { get; set; }

        public OrderStatus Status { get; set; }
        public decimal RemainingAmount { get; set; }  // add this


    }
}
