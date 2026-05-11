using ERP.Services.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.LoginService
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<bool> IsUserActiveAsync(string email);

        Task<(IdentityResult Result, string? UserId)> CreateUserAsync(RegisterViewModel model);
        Task<List<string>> GetAllRolesAsync();
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<IdentityResult> EditUserAsync(EditUserViewModel model);
        Task<EditUserViewModel>? GetByIdAsync(string userId);
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> ToggleUserActiveAsync(string userId);
    }
}
