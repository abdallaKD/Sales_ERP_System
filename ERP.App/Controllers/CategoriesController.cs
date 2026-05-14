using ERP.Domain.Models;
using ERP.Services.CategoryService;
using ERP.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.App.Controllers
{
    //[Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var categories = await _categoryService.GetAllCategoriesAsync();

        //    var viewModel = categories.Select(c => new CategoryViewModel
        //    {
        //        Id = c.Id,
        //        Name = c.Name,
        //        Description = c.Description,
        //        ProductsCount = c.Products?.Count ?? 0,
        //        CreatedAt = c.CreatedAt
        //    }).ToList();

        //    return View("Index", viewModel);
        //}

        // Enhanced Index action with Search and Pagination ===> updated
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(c =>
                    c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int pageSize = 10; 
            int totalItems = categories.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            if (totalPages > 0 && pageNumber > totalPages) pageNumber = totalPages;

            var pagedCategories = categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = pagedCategories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductsCount = c.Products?.Count ?? 0,
                CreatedAt = c.CreatedAt
            }).ToList();

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View("Index", viewModel);
        }


        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductsCount = category.Products?.Count ?? 0,
                Products = category.Products?.ToList()
            };

            return View("Details", viewModel);
        }

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Create", vm);

            try
            {
                var category = new Category
                {
                    Name = vm.Name,
                    Description = vm.Description
                };

                await _categoryService.CreateCategoryAsync(category);
                TempData["Success"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Create", vm);
            }
        }

        #endregion


        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductsCount = category.Products?.Count ?? 0
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel vm)
        {
            if (id != vm.Id)
            {
                TempData["Error"] = "Category ID mismatch!";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View("Edit", vm);

            try
            {
                var category = new Category
                {
                    Id = vm.Id,
                    Name = vm.Name,
                    Description = vm.Description
                };

                await _categoryService.UpdateCategoryAsync(category);
                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Edit", vm);
            }
        }

        #endregion


        #region Delete

        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["Success"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}