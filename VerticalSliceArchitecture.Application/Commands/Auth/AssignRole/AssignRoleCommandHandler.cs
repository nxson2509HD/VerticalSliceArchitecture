using MediatR;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Services;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, StandardResponse>
{
    private readonly IUserService userService;
    private readonly IUserRepository userRepository;

    public AssignRoleCommandHandler(IUserService userService, IUserRepository userRepository)
    {
        this.userService = userService;
        this.userRepository = userRepository;
    }

    public async Task<StandardResponse> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        // Tìm user theo UserName trong request
        var user = await userRepository.GetUserByUserName(request.UserName);
        if (user == null)
        {
            return new StandardResponse
            {
                Success = false,
                Message = $"User '{request.UserName}' was not found",
            };
        }

        // Thêm role cho user
        var rs =  await userService.AssignRoleAsync(user, request.RoleName);

        return new StandardResponse
        {
            Success = true,
            Message = $"Assign role '{request.RoleName}' to user '{request.UserName}' successfully",
        };

    }
}
