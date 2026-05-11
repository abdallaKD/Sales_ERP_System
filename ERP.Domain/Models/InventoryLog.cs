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
    public class InventoryLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Signed quantity:
        ///   +n = IN  (stock increased)
        ///   -n = OUT (stock decreased)
        /// </summary>
        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Movement Type")]
        public InventoryMovementType Type { get; set; }


        /// <summary>
        /// Polymorphic FK — holds the Id of the originating Order or Purchase.
        /// Null for manual adjustments with no linked transaction.
        /// </summary>
 

        [ForeignKey(nameof(Order))]
        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        [ForeignKey(nameof(Purchase))]
        public int? PurchaseId { get; set; }
        public virtual Purchase? Purchase { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation Properties

        [ForeignKey(nameof(Product))]
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }


        [ForeignKey(nameof(CreatedByUser))]
        [Required]
        [Display(Name = "Recorded By")]
        public string CreatedByUserId { get; set; } = string.Empty;
        public virtual ApplicationUser? CreatedByUser { get; set; }

        // ── Factory Methods (not mapped) ─────────────────────────────────────

        /// <summary>Creates an OUT log entry for a fulfilled sales order line.</summary>
        //public static InventoryLog ForSale(int productId, int quantity, int orderId, string userId)
        //    => new()
        //    {
        //        ProductId = productId,
        //        Quantity = -Math.Abs(quantity),   // always negative for OUT
        //        Type = InventoryMovementType.Out,
        //        ReferenceId = orderId,
        //        CreatedByUserId = userId
        //    };

        ///// <summary>Creates an IN log entry for a received purchase line.</summary>
        //public static InventoryLog ForPurchase(int productId, int quantity, int purchaseId, string userId)
        //    => new()
        //    {
        //        ProductId = productId,
        //        Quantity = Math.Abs(quantity),    // always positive for IN
        //        Type = InventoryMovementType.In,
        //        ReferenceId = purchaseId,
        //        CreatedByUserId = userId
        //    };

        ///// <summary>Creates a manual adjustment entry (positive or negative).</summary>
        //public static InventoryLog ForAdjustment(int productId, int signedQuantity, string userId)
        //    => new()
        //    {
        //        ProductId = productId,
        //        Quantity = signedQuantity,
        //        Type = InventoryMovementType.Adjustment,
        //        ReferenceId = null,
        //        CreatedByUserId = userId
        //    };
    }
}
