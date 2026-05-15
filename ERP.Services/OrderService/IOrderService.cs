using ERP.Domain.Models;
using ERP.Services.ViewModels.OrderVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.OrderService
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<OrderViewModel> GetOrdersByCustomerIdAsync(int customerId);
        Task CreateOrderAsync(Order model);
        Task EditOrderAsync(Order model);
        Task CancelOrderAsync(int orderId);
    }
}
