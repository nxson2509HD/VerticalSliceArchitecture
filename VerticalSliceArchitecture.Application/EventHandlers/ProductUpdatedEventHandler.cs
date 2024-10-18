
using AutoMapper;
using Microsoft.Extensions.Logging;
using VerticalSliceArchitecture.Application.Events;
using VerticalSliceArchitecture.Domain.Event;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;


namespace VerticalSliceArchitecture.Application.EventHandlers
{
    public class ProductUpdatedEventHandler : IEventHandler<ProductUpdatedEvent>
    {
        private readonly IProductMongoDbRepository _productMongoRepository;
        private readonly ILogger<ProductUpdatedEventHandler> _logger;
        private readonly IMapper _mapper;

        public ProductUpdatedEventHandler(IProductMongoDbRepository productMongoRepository, ILogger<ProductUpdatedEventHandler> logger, IMapper mapper)
        {
            _productMongoRepository = productMongoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Handle(ProductUpdatedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                var existingProduct = await _productMongoRepository.GetByIdAsync(@event.ProductId);
                if (existingProduct != null)
                {
                   var product = _mapper.Map<ProductMongodb>(@event);

                    await _productMongoRepository.UpdateAsync(product);
                    _logger.LogInformation($"Product with ID {@event.ProductId} has been updated in MongoDB.");
                }
                else
                {
                    _logger.LogWarning($"Product with ID {@event.ProductId} not found in MongoDB.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product with ID {@event.ProductId}: {ex.Message}");
            }
        }
    }
}
