//using ERP.Domain.Models;
//using ERP.Repositories.Repository;
//using ERP.Services.InventoryLogService;
//using ERP.Services.ViewModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ERP.Services.PurchaseService
//{
//    public class PurchaseService : IPurchaseService
//    {
//        private readonly IUnitOfWork unitOfWork;
//        private readonly IInventoryLogService inventoryLogService;

//        public PurchaseService(IUnitOfWork unitOfWork, IInventoryLogService inventoryLogService)
//        {
//            this.unitOfWork = unitOfWork;
//            this.inventoryLogService = inventoryLogService;
//        }

//        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
//        {
//            return await unitOfWork.Purchases.GetAllAsync(p => p.Supplier);
//        }

//        //public async Task<PurchaseViewModel> GetPurchaseByIdAsync(int id)
//        //{
//        //    var purchases = await unitOfWork.Purchases.GetAllAsync(p => p.Supplier);
//        //    var purchase = purchases.FirstOrDefault(p => p.Id == id);

//        //    if (purchase == null) return null;

//        //    return new PurchaseViewModel
//        //    {
//        //        Id = purchase.Id,
//        //        PurchaseDate = purchase.PurchaseDate,
//        //        TotalAmount = purchase.TotalAmount,
//        //        Status = purchase.Status.ToString(),
//        //        SupplierName = purchase.Supplier?.Name ?? "",
//        //        PurchaseItems = purchase.PurchaseDetails.ToList(),
//        //        Suppliers = await unitOfWork.Suppliers.GetAllAsync(),
//        //        Products = await unitOfWork.Products.GetAllAsync()
//        //    };
//        //}
//        public async Task<PurchaseViewModel> GetPurchaseByIdAsync(int id)
//        {
//            var purchases = await unitOfWork.Purchases.FindAsync(
//                p => p.Id == id,
//                includes: ["Supplier", "PurchaseDetails", "PurchaseDetails.Product"]
//            );

//            var purchase = purchases.FirstOrDefault();
//            if (purchase == null) return null;

//            return new PurchaseViewModel
//            {
//                Id = purchase.Id,
//                PurchaseDate = purchase.PurchaseDate,
//                TotalAmount = purchase.TotalAmount,
//                Status = purchase.Status.ToString(),
//                SupplierId = purchase.SupplierId,
//                SupplierName = purchase.Supplier?.Name ?? "",
//                PurchaseItems = purchase.PurchaseDetails.Select(d => new PurchaseItem
//                {
//                    Id = d.Id,
//                    ProductId = d.ProductId,
//                    Product = d.Product,
//                    Quantity = d.Quantity,
//                    UnitCost = d.UnitCost,
//                    PurchaseId = d.PurchaseId
//                }).ToList(),
//                Suppliers = await unitOfWork.Suppliers.GetAllAsync(),
//                Products = await unitOfWork.Products.GetAllAsync()
//            };
//        }

//        public async Task<bool> CreatePurchaseAsync(Purchase purchase, List<PurchaseItem> items)
//        {
//            purchase.PurchaseDetails = items;
//            purchase.TotalAmount = items.Sum(i => i.Quantity * i.UnitCost);

//            await unitOfWork.Purchases.AddAsync(purchase);
//            //return await unitOfWork.CompleteAsync() > 0;
//            bool ret = await unitOfWork.CompleteAsync() > 0;
//            await inventoryLogService.CreateInventoryLogAsync(purchase);
//            return ret;
//        }

//        //public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
//        //{
//        //    Purchase? purchaseLog = await unitOfWork.Purchases.GetByIdAsync(purchase.Id);
//        //    await inventoryLogService.CreateInventoryLogAsync(purchaseLog, true);
//        //    unitOfWork.Purchases.Update(purchase);
//        //    bool ret = await unitOfWork.CompleteAsync() > 0;
//        //    await inventoryLogService.CreateInventoryLogAsync(purchase);
//        //    return ret;
//        //}
//        //public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
//        //{
//        //    var existing = await unitOfWork.Purchases.GetByIdAsync(purchase.Id);
//        //    if (existing is null) return false;

//        //    await inventoryLogService.CreateInventoryLogAsync(existing, true);

//        //    existing.SupplierId = purchase.SupplierId;
//        //    existing.PurchaseDate = purchase.PurchaseDate;
//        //    existing.Status = purchase.Status;
//        //    existing.TotalAmount = purchase.TotalAmount;
//        //    existing.CreatedByUserId = purchase.CreatedByUserId;

//        //    unitOfWork.Purchases.Update(existing);
//        //    bool ret = await unitOfWork.CompleteAsync() > 0;

//        //    await inventoryLogService.CreateInventoryLogAsync(existing);

//        //    return ret;
//        //}
//        public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
//        {
//            var existing = await unitOfWork.Purchases.GetByIdAsync(purchase.Id);
//            if (existing is null) return false;

//            await inventoryLogService.CreateInventoryLogAsync(existing, true);

//            existing.SupplierId = purchase.SupplierId;
//            existing.Status = purchase.Status;
//            existing.CreatedByUserId = purchase.CreatedByUserId;

//            if (purchase.PurchaseDetails != null && purchase.PurchaseDetails.Any())
//            {
//                existing.PurchaseDetails = purchase.PurchaseDetails;
//                existing.TotalAmount = purchase.PurchaseDetails.Sum(i => i.Quantity * i.UnitCost);
//            }

//            unitOfWork.Purchases.Update(existing);
//            bool ret = await unitOfWork.CompleteAsync() > 0;

//            await inventoryLogService.CreateInventoryLogAsync(existing);
//            return ret;
//        }

//        //public async Task<bool> DeletePurchaseAsync(int id)
//        //{
//        //    var result = await unitOfWork.Purchases.FindAsync(p => p.Id == id);
//        //    var purchase = result.FirstOrDefault();

//        //    if (purchase == null) return false;

//        //    unitOfWork.Purchases.Delete(purchase);
//        //    return await unitOfWork.CompleteAsync() > 0;
//        //}
//        public async Task<bool> DeletePurchaseAsync(int id)
//        {
//            var results = await unitOfWork.Purchases.FindAsync(
//                p => p.Id == id,
//                includes: ["PurchaseDetails"]
//            );

//            var purchase = results.FirstOrDefault();
//            if (purchase is null) return false;

//            foreach (var item in purchase.PurchaseDetails)
//                unitOfWork.PurchaseItems.Delete(item);

//            unitOfWork.Purchases.Delete(purchase);

//            return await unitOfWork.CompleteAsync() > 0;
//        }


//        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
//        {
//            return await unitOfWork.Suppliers.GetAllAsync();
//        }

//        public async Task<IEnumerable<Product>> GetAllProductsAsync()
//        {
//            return await unitOfWork.Products.GetAllAsync();
//        }   

//    }
//}
using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.InventoryLogService;
using ERP.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.PurchaseService
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IInventoryLogService inventoryLogService;

        public PurchaseService(IUnitOfWork unitOfWork, IInventoryLogService inventoryLogService)
        {
            this.unitOfWork = unitOfWork;
            this.inventoryLogService = inventoryLogService;
        }

        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await unitOfWork.Purchases.GetAllAsync(p => p.Supplier);
        }

        //public async Task<PurchaseViewModel> GetPurchaseByIdAsync(int id)
        //{
        //    var purchases = await unitOfWork.Purchases.GetAllAsync(p => p.Supplier);
        //    var purchase = purchases.FirstOrDefault(p => p.Id == id);

        //    if (purchase == null) return null;

        //    return new PurchaseViewModel
        //    {
        //        Id = purchase.Id,
        //        PurchaseDate = purchase.PurchaseDate,
        //        TotalAmount = purchase.TotalAmount,
        //        Status = purchase.Status.ToString(),
        //        SupplierName = purchase.Supplier?.Name ?? "",
        //        PurchaseItems = purchase.PurchaseDetails.ToList(),
        //        Suppliers = await unitOfWork.Suppliers.GetAllAsync(),
        //        Products = await unitOfWork.Products.GetAllAsync()
        //    };
        //}
        public async Task<PurchaseViewModel> GetPurchaseByIdAsync(int id)
        {
            var purchases = await unitOfWork.Purchases.FindAsync(
                p => p.Id == id,
                includes: ["Supplier", "PurchaseDetails", "PurchaseDetails.Product"]
            );

            var purchase = purchases.FirstOrDefault();
            if (purchase == null) return null;

            return new PurchaseViewModel
            {
                Id = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                TotalAmount = purchase.TotalAmount,
                Status = purchase.Status.ToString(),
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.Name ?? "",
                PurchaseItems = purchase.PurchaseDetails.Select(d => new PurchaseItem
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    Product = d.Product,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost,
                    PurchaseId = d.PurchaseId
                }).ToList(),
                Suppliers = await unitOfWork.Suppliers.GetAllAsync(),
                Products = await unitOfWork.Products.GetAllAsync()
            };
        }

        public async Task<bool> CreatePurchaseAsync(Purchase purchase, List<PurchaseItem> items)
        {
            purchase.PurchaseDetails = items;
            purchase.TotalAmount = items.Sum(i => i.Quantity * i.UnitCost);

            await unitOfWork.Purchases.AddAsync(purchase);
            bool ret = await unitOfWork.CompleteAsync() > 0;
            await inventoryLogService.CreateInventoryLogAsync(purchase);
            return ret;
        }

        //public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
        //{
        //    Purchase? purchaseLog = await unitOfWork.Purchases.GetByIdAsync(purchase.Id);
        //    await inventoryLogService.CreateInventoryLogAsync(purchaseLog, true);
        //    unitOfWork.Purchases.Update(purchase);
        //    bool ret = await unitOfWork.CompleteAsync() > 0;
        //    await inventoryLogService.CreateInventoryLogAsync(purchase);
        //    return ret;
        //}
        //public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
        //{
        //    var existing = await unitOfWork.Purchases.GetByIdAsync(purchase.Id);
        //    if (existing is null) return false;

        //    await inventoryLogService.CreateInventoryLogAsync(existing, true);

        //    existing.SupplierId = purchase.SupplierId;
        //    existing.PurchaseDate = purchase.PurchaseDate;
        //    existing.Status = purchase.Status;
        //    existing.TotalAmount = purchase.TotalAmount;
        //    existing.CreatedByUserId = purchase.CreatedByUserId;

        //    unitOfWork.Purchases.Update(existing);
        //    bool ret = await unitOfWork.CompleteAsync() > 0;

        //    await inventoryLogService.CreateInventoryLogAsync(existing);

        //    return ret;
        //}
        public async Task<bool> UpdatePurchaseAsync(Purchase purchase)
        {
            // Load existing purchase WITH its items so we can read quantities
            var existingResults = await unitOfWork.Purchases.FindAsync(
                p => p.Id == purchase.Id,
                includes: ["PurchaseDetails", "PurchaseDetails.Product"]
            );
            var existing = existingResults.FirstOrDefault();
            if (existing is null) return false;

            await inventoryLogService.CreateInventoryLogAsync(existing, true);

            bool wasNotReceived = existing.Status != ERP.Domain.Enums.PurchaseStatus.Received;
            bool isNowReceived = purchase.Status == ERP.Domain.Enums.PurchaseStatus.Received;

            existing.SupplierId = purchase.SupplierId;
            existing.Status = purchase.Status;
            existing.CreatedByUserId = purchase.CreatedByUserId;

            if (purchase.PurchaseDetails != null && purchase.PurchaseDetails.Any())
            {
                existing.PurchaseDetails = purchase.PurchaseDetails;
                existing.TotalAmount = purchase.PurchaseDetails.Sum(i => i.Quantity * i.UnitCost);
            }

            // Increase stock only when transitioning into Received for the first time
            if (wasNotReceived && isNowReceived)
            {
                foreach (var item in existing.PurchaseDetails)
                {
                    var product = await unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product is not null)
                    {
                        product.StockQuantity += item.Quantity;
                        unitOfWork.Products.Update(product);
                    }
                }
            }

            unitOfWork.Purchases.Update(existing);
            bool ret = await unitOfWork.CompleteAsync() > 0;

            await inventoryLogService.CreateInventoryLogAsync(existing);
            return ret;
        }

        //public async Task<bool> DeletePurchaseAsync(int id)
        //{
        //    var result = await unitOfWork.Purchases.FindAsync(p => p.Id == id);
        //    var purchase = result.FirstOrDefault();

        //    if (purchase == null) return false;

        //    unitOfWork.Purchases.Delete(purchase);
        //    return await unitOfWork.CompleteAsync() > 0;
        //}
        public async Task<bool> DeletePurchaseAsync(int id)
        {
            var results = await unitOfWork.Purchases.FindAsync(
                p => p.Id == id,
                includes: ["PurchaseDetails"]
            );

            var purchase = results.FirstOrDefault();
            if (purchase is null) return false;

            foreach (var item in purchase.PurchaseDetails)
                unitOfWork.PurchaseItems.Delete(item);

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