using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VerticalSliceArchitecture.Infrastructure.Configuration;
using VerticalSliceArchitecture.Infrastructure.DbContext;
using VerticalSliceArchitecture.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using VerticalSliceArchitecture.Infrastructure.Configurations;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.FileProviders;
using VerticalSliceArchitecture.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddControllers();

// Suppress the default model validation response (use custom validation handling)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Configure KafkaLoggingConfig
builder.Services.Configure<KafkaLoggingConfig>(builder.Configuration.GetSection("kafkaLoggingConfig"));

// Add application and infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Enable directory browsing and static files serving
builder.Services.AddDirectoryBrowser();

// Configure OpenTelemetry
builder.Services.ConfigureOpentelemetry(builder.Configuration.GetValue<string>("otlpUrl"));

// Configure Serilog
builder.Services.ConfigureSerilog(
    builder.Configuration.GetSection(nameof(KafkaLoggingConfig)).Get<KafkaLoggingConfig>(),
    builder.Configuration.GetValue<string>("otlpUrl")
    );

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    // JWT Bearer
    option.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    // API Key
    option.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Description = "API Key needed to access the endpoints."
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
{
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
});
});

builder.Services.AddApiVersioning(
    options =>
    {
        options.ReportApiVersions = true;
    })
.AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Thêm tùy chọn Swagger cho từng phiên bản API
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();


app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });
}
app.UseRateLimiter();
// Ensure the logs directory exists
var logsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "logs");
if (!Directory.Exists(logsPath))
{
    Directory.CreateDirectory(logsPath); // Create the logs directory if it doesn't exist
}
// Enable directory browsing for logs
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(logsPath),
    RequestPath = "/logs"
});
app.UseHttpsRedirection();
app.UseMiddleware<CustomUnauthorizedMiddleware>();

app.UseAuthorization();

app.MapControllers();
//ApplyMigration();
app.Run();


void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}