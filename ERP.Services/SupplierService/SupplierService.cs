using ERP.Domain.Models;
using ERP.Repositories.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.SupplierService
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplierService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
            => await _unitOfWork.Suppliers.GetAllAsync();

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
            => await _unitOfWork.Suppliers.GetByIdAsync(id);

        public async Task<bool> CreateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _unitOfWork.Suppliers.FindAsync(s => s.Email == supplier.Email);
            if (existingSupplier.Any())
                throw new Exception("A supplier with the same email already exists.");

            await _unitOfWork.Suppliers.AddAsync(supplier);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            _unitOfWork.Suppliers.Update(supplier);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.FindAsync(s => s.Id == id, new[] { "Purchases" });
            var supplierData = supplier.FirstOrDefault();

            if (supplierData == null) return false;

            if (supplierData.Purchases.Any())
                throw new Exception("The supplier cannot be deleted because there are purchases registered in his name.");

            _unitOfWork.Suppliers.Delete(supplierData);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
