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
        Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync(); 

        // Details
        Task<OrderDetailsViewModel?> GetOrderDetailsAsync(int id);

        // Create
        Task CreateOrderAsync(OrderDetailsViewModel model , string userId);

        // Edit
        Task UpdateOrderAsync(OrderDetailsViewModel model);

        // cancel
        Task CancelOrderAsync(int id);
    }
}
