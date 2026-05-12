using ERP.Domain.Models;
using ERP.Repositories.Repository;
using ERP.Services.CategoryService;
using ERP.Services.ProductsService;
using ERP.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ERP.Services.ProductsService
{
    public class ProductService : IProductService
    {

        //Dependency Injection
        private readonly IUnitOfWork unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //Product Services

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await unitOfWork.Products.GetAllAsync(p => p.Id);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            var exists = await unitOfWork.Products.FindAsync(p => p.Id == product.Id);

            if (exists == null)
                return false;

            await unitOfWork.Products.AddAsync(product);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await unitOfWork.Products.FindAsync(p => p.Id == id, new[] { "PurchaseItem", "OrderItem", "InventoryLogs" });
            var productData = product.FirstOrDefault();

            if (productData == null) return false;
            if (productData.PurchaseItem.Any() || productData.OrderItem.Any() || productData.InventoryLogs.Any())
            {
                throw new Exception("You Can't Delete this Product. It is already existed in other tables");
            }

            unitOfWork.Products.Delete(productData);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            unitOfWork.Products.Update(product);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsWithCategoryName()
        {
            var products = await unitOfWork.Products.GetAllAsync(p => p.Category);

            return products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Image = p.Image,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            }).ToList();

        }

        public async Task<ProductViewModel> GetProductByIdWithCategoryName(int id)
        {
            var products = await unitOfWork.Products.GetAllAsync(p => p.Category);

            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return null;

            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Image = product.Image,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Categories = await unitOfWork.Categories.GetAllAsync()
            };
        }

    }
}




