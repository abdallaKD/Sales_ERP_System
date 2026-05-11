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
            => await _unitOfWork.Suppliers.GetAllAsync(p => p.Purchases);

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            var suppliers = await _unitOfWork.Suppliers.FindAsync(s => s.Id == id, new[] { "Purchases" });

            return suppliers.FirstOrDefault();
        }

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
            var duplicateEmail = await _unitOfWork.Suppliers.FindAsync(s => s.Email == supplier.Email && s.Id != supplier.Id);
            if (duplicateEmail.Any())
                throw new Exception("Another supplier with the same email already exists.");

            _unitOfWork.Suppliers.Update(supplier);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.FindAsync(s => s.Id == id, new[] { "Purchases" });
            var supplierData = supplier.FirstOrDefault();

            if (supplierData == null) return false;

            if (supplierData.Purchases != null && supplierData.Purchases.Any())
                throw new Exception("The supplier cannot be deleted because it has related purchase records.");

            _unitOfWork.Suppliers.Delete(supplierData);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
