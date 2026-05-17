using ERP.Services.ProductsService;
using Microsoft.AspNetCore.Mvc;
using ERP.Services.ViewModels;
using ERP.Repositories.Repository;
using System.Threading.Tasks;
using ERP.Services.CategoryService;
using ERP.Domain.Models;
using System.Reflection;


namespace ERP.App.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService productService;

        private readonly ICategoryService categoryService;


        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var VM = await productService.GetAllProductsWithCategoryName(pageNumber);

            int pageSize = 5;
            int totalItems = (await productService.GetAllProductsAsync()).Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            return View("Index", VM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProductViewModel
            {
                Categories = await categoryService.GetAllCategoriesAsync()
            };

            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel vm, IFormFile Image)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await categoryService.GetAllCategoriesAsync();
                return View("Create", vm);
            }

            var ImageFile = Image.FileName;
            var product = new Product
            {
                Name = vm.Name,
                SKU = vm.SKU,
                Image = "\\images\\Products\\" + ImageFile,
                StockQuantity = vm.StockQuantity,
                CostPrice = vm.CostPrice,
                SellingPrice = vm.SellingPrice,
                CategoryId = vm.CategoryId
            };

            try
            {
                await productService.CreateProductAsync(product);
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Create", vm);

            }

        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ProductViewModel vm = await productService.GetProductByIdWithCategoryName(id);

            if (vm == null)
                return View("Error");

            return View("Details", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await productService.DeleteProductAsync(id);
                TempData["Success"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ProductViewModel vm = await productService.GetProductByIdWithCategoryName(id);
            if (vm == null)
            {
                TempData["Error"] = "Product not found!";
                return View("Index");
            }

            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel vm, IFormFile? Image)
        {
            if (Image == null || Image.Length == 0)
            {
                ModelState.Remove("Image");
            }

            if (!ModelState.IsValid)
            {
                vm.Categories = await categoryService.GetAllCategoriesAsync();
                return View("Edit", vm);
            }

            var existingProduct = await productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                TempData["Error"] = "Product not found!";
                return RedirectToAction("Index");
            }

            existingProduct.Id = vm.Id;
            existingProduct.Name = vm.Name;
            existingProduct.SKU = vm.SKU;
            existingProduct.Image = Image != null && Image.Length > 0
                ? "\\images\\Products\\" + Image.FileName
                : existingProduct.Image;
            existingProduct.StockQuantity = vm.StockQuantity;
            existingProduct.CostPrice = vm.CostPrice;
            existingProduct.SellingPrice = vm.SellingPrice;
            existingProduct.CategoryId = vm.CategoryId;
            existingProduct.CreatedAt = vm.CreatedAt;


            try
            {
                await productService.UpdateProductAsync(existingProduct);
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Edit", vm);
            }
        }

    }
}
