using MediatR;
using Microsoft.Extensions.Logging;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Services;

public class LoginHandler : IRequestHandler<LoginCommand, StandardResponse>
{
    private readonly IUserService userService;
    private readonly IJwtService jwtService;
    private readonly ILogger<LoginHandler> logger;

    public LoginHandler(IUserService userService, IJwtService jwtService, ILogger<LoginHandler> logger)
    {
        this.userService = userService;
        this.jwtService = jwtService;
        this.logger = logger;
    }

    public async Task<StandardResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Xác thực user
        var user = await userService.AuthenticateAsync(request.Email, request.Password);
        if (user == null)
        {
            return new StandardResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Tạo token JWT
        var token = await jwtService.GenerateJwtToken(user);
        return new StandardResponse
        {
            Success = true,
            Data = token,
            Message = "Login successful"
        };
    }
}
