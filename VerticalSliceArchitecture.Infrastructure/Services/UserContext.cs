using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VerticalSliceArchitecture.Domain.Services;

namespace VerticalSliceArchitecture.Infrastructure.Services
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {

        public string? GetUserId()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string? GetUserName()
        {
            return httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

        public string? GetUserEmail()
        {
            return httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        }

        public bool? IsAuthenticated()
        {
            return httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public bool? IsInRole(string role)
        {
            return httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public List<string> GetUserRoles()
        {
            var claims = httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
            
            if (claims == null || !claims.Any())
            {
                return new List<string>();
            }

            return claims.Select(c => c.Value).ToList();
        }

    }
}
