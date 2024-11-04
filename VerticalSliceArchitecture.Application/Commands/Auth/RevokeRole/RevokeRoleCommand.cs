using MediatR;
using Microsoft.AspNetCore.Identity;
using VerticalSliceArchitecture.Domain.Models;

public class RevokeRoleCommand : IRequest<StandardResponse>
{
    public string? UserName { get; set; } 
    public string? RoleName { get; set; }
}