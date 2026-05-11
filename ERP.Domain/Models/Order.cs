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
    public class Order
    {
        [Key]
        public int Id { get; set; }

        
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

        /// <summary>
        /// Computed: TotalAmount − PaidAmount.
        /// Stored as a persisted computed column in SQL Server; [NotMapped] tells EF
        /// not to write to it directly — configure via HasComputedColumnSql in DbContext.
        /// </summary>
        [NotMapped]
        [Display(Name = "Remaining Amount")]
        public decimal RemainingAmount => TotalAmount - PaidAmount;

        [Required]
        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Display(Name = "Payment Status")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public bool? IsDeleted { get; set; } = false;


        // Navigation Properties

        [ForeignKey(nameof(Customer))]
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }


        [ForeignKey(nameof(CreatedByUser))]
        [Required]
        [Display(Name = "Created By")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual ApplicationUser? CreatedByUser { get; set; }


        public virtual ICollection<OrderItem> OrderDetails { get; set; } = new List<OrderItem>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
