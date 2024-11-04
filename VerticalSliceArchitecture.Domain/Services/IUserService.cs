using Microsoft.AspNetCore.Identity;
using VerticalSliceArchitecture.Domain.Entities;

namespace VerticalSliceArchitecture.Domain.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string email, string password);
        Task<bool> AssignRoleAsync(User user, string role);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<User> RegisterAsync(string email, string password);
        Task<IdentityResult> RevokeRoleAsync(User user, string role);
    }
}
