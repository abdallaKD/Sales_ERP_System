using ERP.Domain.Models;
using ERP.Services.SupplierService;
using ERP.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.App.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();

            var viewModel = suppliers.Select(s => new SupplierViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Phone = s.Phone,
                Email = s.Email,
                PurchasesCount = s.Purchases?.Count ?? 0,
                CreatedAt = s.CreatedAt
            }).ToList();

            return View("Index", viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                TempData["Error"] = "Supplier not found!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                PurchasesCount = supplier.Purchases?.Count ?? 0,
                CreatedAt = supplier.CreatedAt,
                Purchases = supplier.Purchases?.ToList(),
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
        public async Task<IActionResult> Create(SupplierViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Create", vm);

            try
            {
                var supplier = new Supplier
                {
                    Name = vm.Name,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    Address = vm.Address
                };

                await _supplierService.CreateSupplierAsync(supplier);
                TempData["Success"] = "Supplier created successfully!";
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
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                TempData["Error"] = "Supplier not found!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                PurchasesCount = supplier.Purchases?.Count ?? 0
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierViewModel vm)
        {
            if (id != vm.Id)
            {
                TempData["Error"] = "Supplier ID mismatch!";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View("Edit", vm);

            try
            {
                var supplier = new Supplier
                {
                    Id = vm.Id,
                    Name = vm.Name,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    Address = vm.Address
                };

                await _supplierService.UpdateSupplierAsync(supplier);
                TempData["Success"] = "Supplier updated successfully!";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteSupplierAsync(id);
                TempData["Success"] = "Supplier deleted successfully!";
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
