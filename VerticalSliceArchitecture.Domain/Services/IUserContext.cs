namespace VerticalSliceArchitecture.Domain.Services
{
    public interface IUserContext
    {
        string? GetUserId();
        string? GetUserName();
        string? GetUserEmail();
        bool? IsAuthenticated();
        bool? IsInRole(string role);
        List<string>? GetUserRoles();
    }
}
