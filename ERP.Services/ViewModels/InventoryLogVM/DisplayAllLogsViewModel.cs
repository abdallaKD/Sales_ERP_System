using ERP.Domain.Enums;
using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels.InventoryLogVM
{
    public class DisplayAllLogsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Movement Type")]
        public InventoryMovementType Type { get; set; }

        public int? OrderId { get; set; }

        public int? PurchaseId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Product")]
        public string ProductName { get; set; }

        [Display(Name = "Recorded By")]
        public string UserName { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
