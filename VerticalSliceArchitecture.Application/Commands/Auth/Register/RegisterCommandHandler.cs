using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Services;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, StandardResponse>
{

    private readonly ILogger<RegisterCommandHandler> _logger;
    private readonly IUserService _userService;
    private readonly RegisterCommandValidator _validator;
    public RegisterCommandHandler(IUserService userService,ILogger<RegisterCommandHandler> logger,RegisterCommandValidator validator)
    {
        _logger = logger;
        _userService = userService;
        _validator = validator;
    }

    public async Task<StandardResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            return new StandardResponse
            {
                Success = false,
                Message = "Validation failed",
                Data = errors
            };
        }

        // Tạo user
        var user = await _userService.RegisterAsync(request.Email, request.Password);
        return new StandardResponse
        {
            Success = true,
            Message = "User created successfully",
            Data = user
        };
    }
}