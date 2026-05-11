using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Domain.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Status")]
        public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties

        [ForeignKey(nameof(Supplier))]
        [Required]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey(nameof(CreatedByUser))]
        [Required]
        [Display(Name = "Created By")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual ApplicationUser? CreatedByUser { get; set; }

        public virtual ICollection<PurchaseItem> PurchaseDetails { get; set; } = new List<PurchaseItem>();
    }
}
