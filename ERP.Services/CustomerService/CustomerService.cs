using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.ViewModels.CustomerVM;
using ERP.Services.ViewModels.OrderVM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // Read
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _unitOfWork.Customers.GetAllAsync();
        }


        public async Task<Customer>GetCustomerByIdAsync(int id)
        {

            return await _unitOfWork.Customers.GetByIdAsync(id);
        }
       
        
        public async Task CreateCustomerAsync(Customer customer)
        {
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.CompleteAsync();
        }


        public async Task UpdateCustomerAsync(Customer customer)
        {

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.CompleteAsync();
        }


        public async Task<CustomerDetailsViewModel?> GetCustomerDetailsAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return null;

            var orders = await _unitOfWork.Orders.FindAsync(o => o.CustomerId == id);
            var payments = await _unitOfWork.Payments.FindAsync(p => p.CustomerId == id);

            return new CustomerDetailsViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,

                Orders = orders.Select(o => new OrderSummaryViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    RemainingAmount = o.RemainingAmount,
                    Status = o.Status // ← enum على enum مباشرة
                }).ToList(),

                Payments = payments.Select(p => new PaymentSummaryViewModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate
                }).ToList()
            };
        }

       

    }

}
