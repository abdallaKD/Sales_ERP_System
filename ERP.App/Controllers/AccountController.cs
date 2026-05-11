using ERP.Services.LoginService;
using ERP.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            //if (User.Identity?.IsAuthenticated == true)
            //    return RedirectToAction("Index", "Home");

            //ViewData["ReturnUrl"] = returnUrl;
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model, string? returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View("Login", model);

            var result = await _authService.LoginAsync(model);

            if (result.Succeeded)
            {
                //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                //    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Account locked due to multiple failed attempts. Try again later.");
            else if (result.IsNotAllowed)
                ModelState.AddModelError(string.Empty, "Your account is inactive. Contact your administrator.");
            else
                ModelState.AddModelError(string.Empty, "Invalid email or password.");

            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutAsync()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsersAsync()
        {
            var users = await _authService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserAsync()
        {
            var model = new RegisterViewModel
            {
                AvailableRoles = await _authService.GetAllRolesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _authService.GetAllRolesAsync();
                return View(model);
            }

            var (result, _) = await _authService.CreateUserAsync(model);

            if (result.Succeeded)
            {
                TempData["Success"] = "User created successfully.";
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            model.AvailableRoles = await _authService.GetAllRolesAsync();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUserAsync(string id)
        {
            EditUserViewModel model = await _authService.GetByIdAsync(id);
            if (model == null) return NotFound();
            return View("EditUser", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditUserAsync(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _authService.GetAllRolesAsync();
                return View("EditUser", model);
            }
            var result = await _authService.EditUserAsync(model);
            if (result.Succeeded)
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction("Users");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View("EditUser", model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePasswordAsync(string id)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel() { UserId = id };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _authService.ChangePasswordAsync(model);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password reset successfully.";
                return RedirectToAction("Users");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var result = await _authService.DeleteUserAsync(id);
            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? "User deleted." : result.Errors.First().Description;

            return RedirectToAction("Users");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActiveAsync(string id)
        {
            var result = await _authService.ToggleUserActiveAsync(id);
            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? "User status updated." : result.Errors.First().Description;

            return RedirectToAction("Users");
        }
    }

}
