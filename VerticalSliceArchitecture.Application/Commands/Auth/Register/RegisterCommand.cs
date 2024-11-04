using MediatR;
using VerticalSliceArchitecture.Domain.Models;

public class RegisterCommand : IRequest<StandardResponse>
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}