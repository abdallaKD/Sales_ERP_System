using ERP.Domain.Models;
using ERP.Services.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.LoginService
{
    public class AuthService : IAuthService
    {
        private SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var isActive = await IsUserActiveAsync(model.Email);
            if (!isActive)
                return SignInResult.NotAllowed;

            return await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: true
            );
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<bool> IsUserActiveAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is { IsActive: true };
        }

        public async Task<(IdentityResult Result, string? UserId)> CreateUserAsync(RegisterViewModel model)
        {
            ApplicationUser user = new ApplicationUser
            {
                FullName = model.FullName,
                JobTitle = model.JobTitle,
                Email = model.Email,
                UserName = model.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (result, null);

            if (await _roleManager.RoleExistsAsync(model.Role))
                await _userManager.AddToRoleAsync(user, model.Role);

            return (result, user.Id);
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        }

        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            List<UserViewModel> result = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    JobTitle = user.JobTitle,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    CreatedAt = user.CreatedAt
                });
            }
            return result;
        }
        public async Task<IdentityResult> EditUserAsync(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            user.FullName = model.FullName;
            user.JobTitle = model.JobTitle;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }

            var currentRole = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRole);

            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return result;
        }

        public async Task<EditUserViewModel>? GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                JobTitle = user.JobTitle,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                Role = roles.FirstOrDefault() ?? string.Empty,
                AvailableRoles = await GetAllRolesAsync()
            };
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
        }
        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            // Remove roles first
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!removeRolesResult.Succeeded)
                    return removeRolesResult;
            }

            // Remove claims
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any())
            {
                var removeClaimsResult = await _userManager.RemoveClaimsAsync(user, claims);
                if (!removeClaimsResult.Succeeded)
                    return removeClaimsResult;
            }

            // Remove logins
            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var login in logins)
            {
                var removeLoginResult = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                if (!removeLoginResult.Succeeded)
                    return removeLoginResult;
            }

            // Now safe to delete
            return await _userManager.DeleteAsync(user);
        }
        public async Task<IdentityResult> ToggleUserActiveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            user.IsActive = !user.IsActive;
            return await _userManager.UpdateAsync(user);
        }
    }

}
