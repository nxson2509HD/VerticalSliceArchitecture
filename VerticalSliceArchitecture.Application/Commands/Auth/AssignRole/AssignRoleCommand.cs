using MediatR;
using VerticalSliceArchitecture.Domain.Models;

public class AssignRoleCommand : IRequest<StandardResponse>
{
    public string? UserName { get; set; } 
    public string? RoleName { get; set; }
}