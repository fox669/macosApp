using Microsoft.AspNetCore.Identity;
using MacosApp.Web.Data.Entities;
using System.Threading.Tasks;
using MacosApp.web.Data.Entities;
using MacosApp.Web.Models;

namespace MacosApp.Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
