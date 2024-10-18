using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitecture.Domain.Entities;
using VerticalSliceArchitecture.Domain.Services;
using VerticalSliceArchitecture.Infrastructure.DbContext;


public class UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context,
IPasswordHasher<User> passwordHasher) : IUserService
{
    public async Task<User>? AuthenticateAsync(string? email, string? password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == email);

        if (user == null) return null;

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            return user;
        }

        return null;
    }

    public async Task<IList<string>> GetUserRolesAsync(User user)
    {
        return await userManager.GetRolesAsync(user);
    }

    public async Task<User> CreateUserAsync(string email, string password)
    {
        var username = email.Split('@')[0];

        var user = new User
        {
            UserName = username,
            Email = email,
            NormalizedUserName = username.ToUpper(),
            NormalizedEmail = email.ToUpper(),
            EmailConfirmed = false
        };

        await userManager.CreateAsync(user, password);

        await AddUserToRoleAsync(user, "User");

        return user;
    }

    public async Task AddUserToRoleAsync(User user, string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        await userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(User user, string role)
    {
        if (!await userManager.IsInRoleAsync(user, role))
        {
            return IdentityResult.Failed(new IdentityError { Description = $"User is not in the role '{role}'." });
        }

        return await userManager.RemoveFromRoleAsync(user, role);
    }
}
