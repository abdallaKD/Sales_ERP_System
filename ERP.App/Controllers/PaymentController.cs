using ERP.Domain.Models;
using ERP.Services.PaymentService;
using ERP.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ERP.App.Controllers
{
    public class PaymentController : Controller
    {
        IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            this._paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> OrderPayments(int orderId)
        {
            var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            ViewBag.OrderId = orderId;
            return View(payments);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePayment(int orderId)
        {
            var model = await _paymentService.GetPaymentFormAsync(orderId);
            if (model is null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await _paymentService.GetPaymentFormAsync(model.OrderId);
                if (refreshed is not null)
                {
                    model.CustomerName = refreshed.CustomerName;
                    model.TotalAmount = refreshed.TotalAmount;
                    model.PaidAmount = refreshed.PaidAmount;
                }
                return View(model);
            }

            var (success, message) = await _paymentService.CreatePaymentAsync(model);
            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction("OrderPayments", new { orderId = model.OrderId });
            }

            ModelState.AddModelError(string.Empty, message);
            var form = await _paymentService.GetPaymentFormAsync(model.OrderId);
            if (form is not null)
            {
                model.CustomerName = form.CustomerName;
                model.TotalAmount = form.TotalAmount;
                model.PaidAmount = form.PaidAmount;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePaymentAsync(int id, int orderId)
        {
            var (success, message) = await _paymentService.DeletePaymentAsync(id);

            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction("OrderPayments", new { orderId });
        }
    }
}
