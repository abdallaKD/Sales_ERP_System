using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ERP.Services.OrderService;
using ERP.Services.ViewModels.OrderVM;
using ERP.Repositories.Repository;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Admin,SalesEmployee")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
        }
        #region help action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRow(OrderDetailsViewModel model)
        {
            model.Items ??= new List<OrderItemFormViewModel>();
            model.Items.Add(new OrderItemFormViewModel());

            await BuildFormViewModel(model);

            return View("Create", model);
        }

        #endregion





        // ── INDEX ──────────────────────────────────────────
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            // Fetch all orders from service
            var orders = await _orderService.GetAllOrdersAsync();

            // Apply search filter (case‑insensitive, by customer name)
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Pagination setup
            int pageSize = 5;
            int totalItems = orders.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Validate page number
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            if (totalPages > 0 && pageNumber > totalPages) pageNumber = totalPages;

            // Apply pagination
            var pagedOrders = orders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to ViewModel (OrderViewModel)
            var viewModel = pagedOrders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                PaidAmount = o.PaidAmount,
                RemainingAmount = o.RemainingAmount
                // adjust any other properties you have
            }).ToList();

            // Pass pagination metadata via ViewData
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View(viewModel);
        }

        // ── DETAILS ────────────────────────────────────────
        public async Task<IActionResult> Details(int id)
        {
            var model = await _orderService.GetOrderDetailsAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // ── CREATE GET ─────────────────────────────────────
       
        public async Task<IActionResult> Create()
        {
            var model = new OrderDetailsViewModel
            {
                Items = new List<OrderItemFormViewModel> { new() }
            };
            return View(await BuildFormViewModel(model));
        }
       
        // ── CREATE POST ────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetailsViewModel model)
        {
            // filter empty rows BEFORE any validation
            model.Items = model.Items
                .Where(i => i.ProductId > 0)
                .ToList();

            if (model.CustomerId == 0)
                ModelState.AddModelError("CustomerId", "Please select a customer.");

            if (!model.Items.Any())
                ModelState.AddModelError("", "Please select at least one product.");

            if (!ModelState.IsValid)
                return View(await BuildFormViewModel(model));

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _orderService.CreateOrderAsync(model, userId);
                TempData["Success"] = "Order created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(await BuildFormViewModel(model));
            }
        }




        // ── EDIT GET ───────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _orderService.GetOrderDetailsAsync(id);
            if (model == null) return NotFound();
            return View(await BuildFormViewModel(model));
        }

       
        // ── EDIT POST ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderDetailsViewModel model)
        {
            model.Items = model.Items
                .Where(i => i.ProductId > 0)
                .ToList();

            if (model.CustomerId == 0)
                ModelState.AddModelError("CustomerId", "Please select a customer.");

            if (!model.Items.Any())
                ModelState.AddModelError("", "Please select at least one product.");

            if (!ModelState.IsValid)
                return View(await BuildFormViewModel(model));

            try
            {
                await _orderService.UpdateOrderAsync(model);
                TempData["Success"] = "Order updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(await BuildFormViewModel(model));
            }
        }
       
        
        
        // ── CANCEL GET ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);

            if (order == null)
                return NotFound();

            return View(order);
        }
        // ── CANCEL POST ────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancel(int id)
        {
            try
            {
                await _orderService.CancelOrderAsync(id);
                TempData["Success"] = "Order cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ── PRIVATE HELPER ─────────────────────────────────
        private async Task<OrderDetailsViewModel> BuildFormViewModel(OrderDetailsViewModel model)
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var products = await _unitOfWork.Products.GetAllAsync();

            model.Customers = new SelectList(customers, "Id", "Name", model.CustomerId);
            model.Products = new SelectList(
                products.Select(p => new {
                    p.Id,
                    Display = $"{p.Name} (Stock: {p.StockQuantity})"
                }),
                "Id", "Display"
            );

            return model;
        }
    }
}