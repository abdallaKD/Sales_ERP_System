using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.ViewModels;

namespace ERP.Services.PurchaseService
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork unitOfWork;

        public PurchaseService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await unitOfWork.Purchases.GetAllAsync(p => p.Supplier);
        }

        public async Task<PurchaseViewModel> GetPurchaseByIdAsync(int id)
        {
            var purchases = await unitOfWork.Purchases.GetAllAsync(p => p.Supplier);
            var purchase = purchases.FirstOrDefault(p => p.Id == id);

            if (purchase == null) return null;

            return new PurchaseViewModel
            {
                Id = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                TotalAmount = purchase.TotalAmount,
                Status = purchase.Status.ToString(),
                SupplierName = purchase.Supplier?.Name ?? "",
                PurchaseItems = purchase.PurchaseDetails.ToList(),
                Suppliers = await unitOfWork.Suppliers.GetAllAsync(),
                Products = await unitOfWork.Products.GetAllAsync()
            };
        }

        public async Task<bool> CreatePurchaseAsync(Purchase purchase, List<PurchaseItem> items)
        {
            purchase.PurchaseDetails = items;
            purchase.TotalAmount = items.Sum(i => i.Quantity * i.UnitCost);

            await unitOfWork.Purchases.AddAsync(purchase);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
        {
            unitOfWork.Purchases.Update(purchase);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeletePurchaseAsync(int id)
        {
            var result = await unitOfWork.Purchases.FindAsync(p => p.Id == id);
            var purchase = result.FirstOrDefault();

            if (purchase == null) return false;

            unitOfWork.Purchases.Delete(purchase);
            return await unitOfWork.CompleteAsync() > 0;
        }


        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await unitOfWork.Suppliers.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await unitOfWork.Products.GetAllAsync();
        }

    }
}
