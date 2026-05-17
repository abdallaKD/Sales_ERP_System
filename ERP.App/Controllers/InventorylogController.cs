using Microsoft.AspNetCore.Mvc;
using ERP.Services.ViewModels.InventoryLogVM;
using ERP.Services.InventoryLogService;
using ERP.Services.ProductsService;
using ERP.Services.LoginService;
using ERP.Domain.Models;
using ERP.Repositories.Repository;

namespace ERP.App.Controllers
{
    public class InventorylogController : Controller
    {
        private readonly IInventoryLogService inventoryLogService;
        private readonly IProductService productService;
        private readonly IAuthService authService;

        public InventorylogController(IInventoryLogService inventoryLogService, IProductService productService, IAuthService authService)
        {
            this.inventoryLogService = inventoryLogService;
            this.productService = productService;
            this.authService = authService;
        }



        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            List<DisplayAllLogsViewModel> vm = new List<DisplayAllLogsViewModel>();

            var Logs = await inventoryLogService.GetAllInventoryLogsAsync();


            int pageSize = 5;
            int totalItems = Logs.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            if (totalPages > 0 && pageNumber > totalPages) pageNumber = totalPages;

            var pagedLogs = Logs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();



            foreach (var LogsItem in pagedLogs)
            {
                vm.Add(new DisplayAllLogsViewModel()
                {
                    Id = LogsItem.Id,
                    Quantity = LogsItem.Quantity,
                    Type = LogsItem.Type,
                    OrderId = LogsItem.OrderId,
                    PurchaseId = LogsItem.PurchaseId,
                    CreatedAt = LogsItem.CreatedAt,
                    ProductName = (await productService.GetProductByIdAsync(LogsItem.ProductId)).Name,
                    UserName = (await authService.GetByIdAsync(LogsItem.CreatedByUserId)).FullName,
                    UserId = LogsItem.CreatedByUserId
                });
            }

            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View("Index", vm);

        }


        public async Task<IActionResult> Details(int id)
        {
            InventoryLog LogsItem = await inventoryLogService.GetInventoryLogByIdAsync(id);


            DisplayAllLogsViewModel vm = new DisplayAllLogsViewModel()
            {
                Id = LogsItem.Id,
                Quantity = LogsItem.Quantity,
                Type = LogsItem.Type,
                OrderId = LogsItem.OrderId,
                PurchaseId = LogsItem.PurchaseId,
                CreatedAt = LogsItem.CreatedAt,
                ProductName = (await productService.GetProductByIdAsync(LogsItem.ProductId)).Name,
                UserName = (await authService.GetByIdAsync(LogsItem.CreatedByUserId)).FullName,
                UserId = LogsItem.CreatedByUserId
            };

            return View("Details", vm);


        }

    }
}
