using ERP.Domain.Models;
using ERP.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.ProductsService
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<ProductViewModel>> GetAllProductsWithCategoryName();
        Task<ProductViewModel> GetProductByIdWithCategoryName(int id);
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
