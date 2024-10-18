using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using MongoDB.Driver;
using VerticalSliceArchitecture.Domain.Entities;
using VerticalSliceArchitecture.Infrastructure.Services;
using VerticalSliceArchitecture.Infrastructure.DbContext;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Infrastructure.Repositories;
using VerticalSliceArchitecture.Domain.Services;
using VerticalSliceArchitecture.Domain.Event;
using VerticalSliceArchitecture.Application.Events;
using VerticalSliceArchitecture.Application.EventHandlers;
using FluentValidation;
using FluentValidation.AspNetCore;
using VerticalSliceArchitecture.Infrastructure.Auth;

namespace VerticalSliceArchitecture.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("VerticalSliceArchitecture");
            var kafkaBootstrapServers = configuration.GetSection("kafkaLoggingConfig:bootstrapServers").Value;
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            var mongoConnectionString = configuration.GetSection("MongoDB:ConnectionString").Value;
            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
            services.AddSingleton<IMongoClient>(mongoClient);

            // Cấu hình Connection Pool
            mongoClientSettings.MaxConnectionPoolSize = 100;

            // Cấu hình Timeout
            mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(20);
            mongoClientSettings.SocketTimeout = TimeSpan.FromSeconds(20);

            services.AddIdentityApiEndpoints<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            // Configure JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            })
           .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("apiKey", options => { });

            // // Cấu hình Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("JwtOrApiKey", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "apiKey");
                    policy.RequireAuthenticatedUser();
                });
            });
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductMongoDbRepository, ProductMongoDbRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Register Kafka Producer Service
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

            // Register Kafka Consumer as Hosted Service
            services.AddHostedService<KafkaConsumerService>();
            services.AddScoped<IEventHandler<ProductCreatedEvent>, ProductCreatedEventHandler>();
            services.AddScoped<IEventHandler<ProductDeletedEvent>, ProductDeletedEventHandler>();
            services.AddScoped<IEventHandler<ProductUpdatedEvent>, ProductUpdatedEventHandler>();

            // Register Redis Service
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddScoped<IRedisCacheService, RedisCacheService>();
        }

        public static void AddApplication(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

            services.AddAutoMapper(applicationAssembly);

            services.AddValidatorsFromAssembly(applicationAssembly)
                .AddFluentValidationAutoValidation();
        }
    }

}