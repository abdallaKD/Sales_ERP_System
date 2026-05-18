using ERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalProfit { get; set; }
        public List<DashboardOrderRow> RecentOrders { get; set; } = new();
        public List<LowStockProductRow> LowStockProducts { get; set; } = new();
    }

    public class DashboardOrderRow
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }

    public class LowStockProductRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
