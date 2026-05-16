using ERP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Services.ViewModels;

namespace ERP.Services.PurchaseService
{
    public interface IPurchaseService
    {
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task<PurchaseViewModel> GetPurchaseByIdAsync(int id);
        Task<bool> CreatePurchaseAsync(Purchase purchase, List<PurchaseItem> items);
        Task<bool> UpdatePurchaseAsync(Purchase purchase);
        Task<bool> DeletePurchaseAsync(int id);

        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<IEnumerable<Product>> GetAllProductsAsync();
    }
}
