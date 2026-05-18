using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.ViewModels;
using ERP.Services.ViewModels.CustomerVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<(bool Success, string Message)> CreatePaymentAsync(PaymentViewModel model);
        Task<List<PaymentListViewModel>> GetAllPaymentsAsync();
        Task<List<PaymentListViewModel>> GetPaymentsByOrderIdAsync(int orderId);
        Task<(bool Success, string Message)> DeletePaymentAsync(int paymentId);
        Task<PaymentViewModel?> GetPaymentFormAsync(int orderId);
    }
}
