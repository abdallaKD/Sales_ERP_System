using ERP.Domain.Models;
using ERP.Services.CategoryService;
using ERP.Services.CustomerService;
using ERP.Services.ViewModels.CustomerVM;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static NuGet.Packaging.PackagingConstants;

namespace ERP.App.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        // GET: /Customer
        public async Task<IActionResult> Index()
        {

            var customers = await _customerService.GetAllCustomersAsync();
            var customerVMs = customers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
            });

            return View(customerVMs);
        }
        #region create
        //Get :  /Customer/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CustomerViewModel());


        }


        // POST: /Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CustomerViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Create", vm);

            try
            {
                var customer = new Customer
                {
                    Name = vm.Name,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    Address = vm.Address,

                };

                await _customerService.CreateCustomerAsync(customer);
                TempData["Success"] = "Customer created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Create", vm);
            }
        }




        #endregion

        #region Details

        // GET: /Customer/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _customerService.GetCustomerDetailsAsync(id);
            if (viewModel == null)
            {
                TempData["Error"] = "Customer not found!";
                return RedirectToAction(nameof(Index));
            }
            // CustomerDetailsViewModel
            return View(viewModel);
        }

        #endregion



        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found!";
                return RedirectToAction(nameof(Index));
            }

            customer.Name = vm.Name;
            customer.Phone = vm.Phone;
            customer.Email = vm.Email;
            customer.Address = vm.Address;

            await _customerService.UpdateCustomerAsync(customer);

            TempData["Success"] = "Customer updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        #endregion



        #region delete



        // GET: /Customer/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found!";
                return RedirectToAction(nameof(Index));
            }

            var vm = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return View(vm);
        }
        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                TempData["Success"] = "Customer deleted successfully!";
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
