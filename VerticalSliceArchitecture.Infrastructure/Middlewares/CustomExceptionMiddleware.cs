using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VerticalSliceArchitecture.Domain.Models;

namespace VerticalSliceArchitecture.Infrastructure.Middlewares;

public class CustomUnauthorizedMiddleware
{
    private readonly RequestDelegate _next;

    public CustomUnauthorizedMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            context.Response.ContentType = "application/json";

            var response = new StandardResponse
            {
                Success = false,
                Message = "Unauthorized access",
                ExceptionMessage = "You are not authorized to access this resource."
            };

            var jsonResponse = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
