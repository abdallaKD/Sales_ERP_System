using ERP.Domain.Enums;
using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message)> CreatePaymentAsync(PaymentViewModel model)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.Id == model.OrderId, includes: ["Customer"]);

            var order = orders.FirstOrDefault();
            if (order is null) return (false, "Order not found");

            if (order.Status == OrderStatus.Cancelled) return (false, "Cannot add payment to a cancelled order");

            var remaining = order.TotalAmount - order.PaidAmount;
            if (model.Amount > remaining)
                return (false, $"Payment amount ({model.Amount:C}) exceeds the remaining balance ({remaining:C}).");

            var payment = new Payment
            {
                OrderId = model.OrderId,
                CustomerId = order.CustomerId,
                Amount = model.Amount,
                PaymentDate = model.PaymentDate,
                PaymentMethod = model.PaymentMethod
            };

            await _unitOfWork.Payments.AddAsync(payment);

            order.PaidAmount += model.Amount;
            order.PaymentStatus = order.PaidAmount >= order.TotalAmount ? PaymentStatus.Paid : PaymentStatus.Partial;
            _unitOfWork.Orders.Update(order);

            await _unitOfWork.CompleteAsync();

            return (true, "Payment added successfully");
        }

        public async Task<List<PaymentListViewModel>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.Payments.FindAsync(p => true, includes: ["Order", "Order.Customer"]);

            return payments.Select(p => new PaymentListViewModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                CustomerName = p.Order?.Customer?.Name,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod.ToString(),
                OrderTotal = p.Order?.TotalAmount ?? 0,
                OrderPaid = p.Order?.PaidAmount ?? 0,
                PaymentStatus = p.Order?.PaymentStatus.ToString() ?? string.Empty
            }).ToList();
        }

        public async Task<List<PaymentListViewModel>> GetPaymentsByOrderIdAsync(int orderId)
        {
            var payments = await _unitOfWork.Payments.FindAsync(p => p.OrderId == orderId, includes: ["Order", "Order.Customer"]);
            return payments.Select(p => new PaymentListViewModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                CustomerName = p.Order?.Customer?.Name,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod.ToString(),
                OrderTotal = p.Order?.TotalAmount ?? 0,
                OrderPaid = p.Order?.PaidAmount ?? 0,
                PaymentStatus = p.Order?.PaymentStatus.ToString() ?? string.Empty
            }).ToList();
        }

        public async Task<PaymentViewModel?> GetPaymentFormAsync(int orderId)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.Id == orderId, includes: ["Customer"]);

            var order = orders.FirstOrDefault();
            if (order is null) return null;

            return new PaymentViewModel
            {
                OrderId = order.Id,
                CustomerName = order.Customer?.Name,
                TotalAmount = order.TotalAmount,
                PaidAmount = order.PaidAmount,
                PaymentDate = DateTime.Now
            };
        }

        public async Task<(bool Success, string Message)> DeletePaymentAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment is null)
                return (false, "Payment not found");

            var orders = await _unitOfWork.Orders.FindAsync(o => o.Id == payment.OrderId);
            var order = orders.FirstOrDefault();

            if (order is null)
                return (false, "Order not found");

            if (order.Status == OrderStatus.Completed) return (false, "Sorry, Order status is completed");

            order.PaidAmount -= payment.Amount;
            if (order.PaidAmount < 0) order.PaidAmount = 0;

            order.PaymentStatus = order.PaidAmount <= 0 ? PaymentStatus.Pending : order.PaidAmount >= order.TotalAmount ? PaymentStatus.Paid : PaymentStatus.Partial;

            _unitOfWork.Payments.Delete(payment);
            _unitOfWork.Orders.Update(order);

            await _unitOfWork.CompleteAsync();

            return (true, "Payment deleted and order balance reversed.");
        }

    }
}
