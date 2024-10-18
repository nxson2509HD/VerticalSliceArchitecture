using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace VerticalSliceArchitecture.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigExtensions(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ServiceExtensions).Assembly;

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

            services.AddAutoMapper(applicationAssembly);

            services.AddValidatorsFromAssembly(applicationAssembly)
                .AddFluentValidationAutoValidation();
        }
    }
}
