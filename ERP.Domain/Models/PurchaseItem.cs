using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Models
{
    public class PurchaseItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        [NotMapped]
        [Display(Name = "Total Cost")]
        public decimal TotalCost => Quantity * UnitCost;

        //Navigation Properties

        [ForeignKey(nameof(Purchase))]
        [Required]
        public int PurchaseId { get; set; }
        public virtual Purchase? Purchase { get; set; }


        [ForeignKey(nameof(Product))]
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
