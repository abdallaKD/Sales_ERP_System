using Microsoft.AspNetCore.Mvc;
using ERP.Domain.Models;
using ERP.Domain.Enums;
using ERP.Services.PurchaseService;
using ERP.Services.ViewModels;


namespace ERP.App.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            this.purchaseService = purchaseService;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await purchaseService.GetAllPurchasesAsync();
            return View("Index", purchases);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vm = await purchaseService.GetPurchaseByIdAsync(id);
            if (vm == null) return View("Error");
            return View("Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new PurchaseViewModel
            {
                Suppliers = await purchaseService.GetAllSuppliersAsync(),
                Products = await purchaseService.GetAllProductsAsync()
            };
            return View("Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Suppliers = await purchaseService.GetAllSuppliersAsync();
                vm.Products = await purchaseService.GetAllProductsAsync();
                return View("Create", vm);
            }

            var purchase = new Purchase
            {
                SupplierId = vm.SupplierId,
                PurchaseDate = vm.PurchaseDate,
                Status = PurchaseStatus.Pending,
            };

            var items = vm.PurchaseItems.Select(i => new PurchaseItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitCost = i.UnitCost
            }).ToList();

            try
            {
                await purchaseService.CreatePurchaseAsync(purchase, items);
                TempData["Success"] = "Purchase created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Create", vm);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await purchaseService.GetPurchaseByIdAsync(id);
            if (vm == null)
            {
                TempData["Error"] = "Purchase not found!";
                return RedirectToAction("Index");
            }
            return View("Edit", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Edit", vm);

            var purchase = new Purchase
            {
                Id = id,
                SupplierId = vm.SupplierId,
                PurchaseDate = vm.PurchaseDate,
                Status = Enum.Parse<PurchaseStatus>(vm.Status),
                TotalAmount = vm.TotalAmount,
                CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
            };

            try
            {
                await purchaseService.UpdatePurchaseAsync(purchase);
                TempData["Success"] = "Purchase updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Edit", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await purchaseService.DeletePurchaseAsync(id);
                TempData["Success"] = "Purchase deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }




    }
}
