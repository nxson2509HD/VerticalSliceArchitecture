using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitecture.Domain.Entities;
using VerticalSliceArchitecture.Domain.Services;
using VerticalSliceArchitecture.Infrastructure.DbContext;


public class UserService : IUserService
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    private AppDbContext _context;
    private IPasswordHasher<User> _passwordHasher;
    public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context,IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _passwordHasher = passwordHasher;
            
    }
    public async Task<User>? AuthenticateAsync(string? email, string? password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == email);

        if (user == null) return null;

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            return user;
        }

        return null;
    }

    public async Task<IList<string>> GetUserRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<User> RegisterAsync(string email, string password)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            NormalizedUserName = email.ToUpper(),
            NormalizedEmail = email.ToUpper(),
            EmailConfirmed = false
        };

        await _userManager.CreateAsync(user, password);

        await AssignRoleAsync(user, "User");

        return user;
    }

    public async Task<bool> AssignRoleAsync(User user, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        await _userManager.AddToRoleAsync(user, role);
        return true;
    }

    public async Task<IdentityResult> RevokeRoleAsync(User user, string role)
    {
        if (!await _userManager.IsInRoleAsync(user, role))
        {
            return IdentityResult.Failed(new IdentityError { Description = $"User is not in the role '{role}'." });
        }

        return await _userManager.RemoveFromRoleAsync(user, role);
    }
}
