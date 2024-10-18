using VerticalSliceArchitecture.Domain.Entities;

namespace VerticalSliceArchitecture.Domain.Services
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(User user);
    }
}
