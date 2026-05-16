using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels
{
    public class PurchaseViewModel
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string CreatedByUser { get; set; } = string.Empty;
        public List<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public IEnumerable<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        public int SupplierId { get; set; }
    }
}
