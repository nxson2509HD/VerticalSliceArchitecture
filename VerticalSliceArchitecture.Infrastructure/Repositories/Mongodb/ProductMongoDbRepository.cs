using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;

namespace VerticalSliceArchitecture.Infrastructure.Repositories
{
    public class ProductMongoDbRepository : IProductMongoDbRepository
    {
        private readonly IMongoCollection<ProductMongodb> _productsCollection;
        private readonly AsyncPolicy _policy;
        private readonly ILogger<ProductMongoDbRepository> _logger;

        public ProductMongoDbRepository(IMongoClient mongoClient, IConfiguration configuration, ILogger<ProductMongoDbRepository> logger)
        {
            var mongoDatabase = mongoClient.GetDatabase(configuration.GetSection("MongoDB:DatabaseName").Value);
            _productsCollection = mongoDatabase.GetCollection<ProductMongodb>("Products");

            _logger = logger;

            var retryPolicy = Policy
                .Handle<MongoException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} due to {exception.Message}");
                    }
                );

            var circuitBreakerPolicy = Policy
                .Handle<MongoException>()
                .Or<TaskCanceledException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, breakDelay) =>
                    {
                        _logger.LogWarning($"Circuit breaker opened for {breakDelay.TotalSeconds} seconds due to {exception.Message}");
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker reset.");
                    }
                );

            // Kết hợp Retry với Circuit Breaker
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(30));
            _policy = Policy.WrapAsync(timeoutPolicy, retryPolicy, circuitBreakerPolicy);
        }

        public async Task<ProductMongodb?> GetByIdAsync(int id)
        {
            return await _policy.ExecuteAsync(async () =>
            {
                return await _productsCollection
                    .Find(p => p.ProductId == id)
                    .FirstOrDefaultAsync();
            });
        }

        public async Task<int> CreateAsync(ProductMongodb product)
        {
            await _policy.ExecuteAsync(async () =>
            {
                await _productsCollection.InsertOneAsync(product);
            });

            return product.ProductId;
        }

        public async Task DeleteAsync(int id)
        {
            await _policy.ExecuteAsync(async () =>
            {
                var deleteResult = await _productsCollection.DeleteOneAsync(p => p.ProductId == id);
            });
        }

        public async Task UpdateAsync(ProductMongodb product)
        {
            await _policy.ExecuteAsync(async () =>
            {
                await _productsCollection.ReplaceOneAsync(p => p.ProductId == product.ProductId, product);
            });
        }
    }

}
