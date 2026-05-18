using ERP.Domain.Enums;
using ERP.Repositories.Repository;
using ERP.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var orders = await _unitOfWork.Orders.FindAsync(o => true, includes: ["Customer"]);
            var products = await _unitOfWork.Products.GetAllAsync();

            var totalSales = orders
                .Where(o => o.Status == OrderStatus.Completed)
                .Sum(o => o.TotalAmount);

            var orderItemsWithProducts = await _unitOfWork.OrderItems.FindAsync(i => true, includes: ["Product"]);
            var totalRevenue = orderItemsWithProducts
                .Where(i => i.Order == null || i.Order.Status == OrderStatus.Completed)
                .Sum(i => i.Quantity * i.UnitPrice);

            var totalCost = orderItemsWithProducts
                .Where(i => i.Order == null || i.Order.Status == OrderStatus.Completed)
                .Sum(i => i.Quantity * (i.Product?.CostPrice ?? 0));

            var totalProfit = totalRevenue - totalCost;

            var recentOrders = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new DashboardOrderRow
                {
                    Id = o.Id,
                    CustomerName = o.Customer?.Name ?? "—",
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    PaidAmount = o.PaidAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus
                }).ToList();

            var lowStockProducts = products
                .OrderBy(p => p.StockQuantity)
                .Take(10)
                .Select(p => new LowStockProductRow
                {
                    Id = p.Id,
                    Name = p.Name,
                    StockQuantity = p.StockQuantity
                }).ToList();

            return new DashboardViewModel
            {
                TotalCustomers = customers.Count(),
                TotalSales = totalSales,
                TotalOrders = orders.Count(),
                TotalProfit = totalProfit,
                RecentOrders = recentOrders,
                LowStockProducts = lowStockProducts
            };
        }
    }
}
