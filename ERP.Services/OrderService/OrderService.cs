using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.ViewModels.OrderVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.OrderService
{
    public class OrderService : IOrderService

    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;



        public async  Task<IEnumerable<Order>> GetAllOrdersAsync()
        {

            return await _unitOfWork.Orders.GetAllAsync();
        }


        public  async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _unitOfWork.Orders.GetByIdAsync(id);
        }



        public Task CancelOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrderAsync(Order model)
        {
            throw new NotImplementedException();
        }

        public Task EditOrderAsync(Order model)
        {
            throw new NotImplementedException();
        }

        public Task<OrderViewModel> GetOrdersByCustomerIdAsync(int customerId)
        {
            throw new NotImplementedException();
        }





        //public Task<OrderViewModel> GetOrdersByCustomerIdAsync(int customerId)
        //{
        //    var orders = await _unitOfWork.Orders.FindAsync(o => o.CustomerId == customerId);
        //    return orders.Select(o => new OrderDetailsViewModel
        //    {
        //        OrderId = o.Id,
        //        CustomerName = o.Customer.Name,
        //        OrderDate = o.OrderDate,
        //        Status = o.Status,
        //        TotalAmount = o.TotalAmount,
        //        RemainingAmount = o.RemainingAmount,
        //        Items = o.OrderItems.Select(oi => new OrderItemDetailsViewModel
        //        {
        //            ProductName = oi.Product.Name,
        //            Quantity = oi.Quantity,
        //            UnitPrice = oi.UnitPrice,
        //            LineTotal = oi.LineTotal
        //        }).ToList()
        //    });
        //}
    }
}
