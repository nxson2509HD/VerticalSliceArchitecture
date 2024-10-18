using Microsoft.AspNetCore.Identity;
using VerticalSliceArchitecture.Domain.Entities;

namespace VerticalSliceArchitecture.Domain.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateAsync(string email, string password);
        Task AddUserToRoleAsync(User user, string role);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<User> CreateUserAsync(string email, string password);
        Task<IdentityResult> RemoveUserFromRoleAsync(User user, string role);
    }
}
