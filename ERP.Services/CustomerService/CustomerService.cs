using ERP.Domain.Models;
using ERP.Repositories.Repository;
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

       
    }
}
