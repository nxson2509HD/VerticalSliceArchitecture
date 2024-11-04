using MediatR;
using Microsoft.AspNetCore.Identity;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Services;

public class RevokeRoleCommandHandler(IUserService userService,
IUserRepository userRepository) : IRequestHandler<RevokeRoleCommand, StandardResponse>
{

    public async Task<StandardResponse> Handle(RevokeRoleCommand request, CancellationToken cancellationToken)
    {
        // Tìm user theo username
            var user = await userRepository.GetUserByUserName(request.UserName);
            if (user == null)
            {
                return new StandardResponse
                {
                    Success = false,
                    Message = $"User '{request.UserName}' was not found"
                };
            }

            // Xóa role khỏi user
            var result = await userService.RevokeRoleAsync(user, request.RoleName);
            if (!result.Succeeded)
            {
                return new StandardResponse
                {
                    Success = false,
                    Message = "Failed to remove role",
                    ErrorData = result.Errors
                };
            }

            return new StandardResponse
            {
                Success = true,
                Message = $"Role '{request.RoleName}' removed from user successfully",
                Data = request.RoleName
            };
    }
}