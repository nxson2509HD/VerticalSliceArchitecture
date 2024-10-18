using AutoMapper;
using Microsoft.Extensions.Logging;
using VerticalSliceArchitecture.Application.Events;
using VerticalSliceArchitecture.Domain.Event;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;


namespace VerticalSliceArchitecture.Application.EventHandlers
{
    public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
    {
        private readonly IProductMongoDbRepository _productMongoRepository;
        private readonly ILogger<ProductCreatedEventHandler> _logger;
        private readonly IMapper _mapper;

        public ProductCreatedEventHandler(IProductMongoDbRepository productMongoRepository, ILogger<ProductCreatedEventHandler> logger, IMapper mapper)
        {
            _productMongoRepository = productMongoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Handle(ProductCreatedEvent @event, CancellationToken cancellationToken)
        {
            try
            {
                var existingProduct = await _productMongoRepository.GetByIdAsync(@event.ProductId);
                if (existingProduct == null)
                {
                    var product = _mapper.Map<ProductMongodb>(@event);

                    await _productMongoRepository.CreateAsync(product);
                    
                    _logger.LogInformation($"Product with ID {@event.ProductId} has been created in MongoDB.");
                }
                else
                {
                    _logger.LogInformation($"Product with ID {@event.ProductId} already exists in MongoDB.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating product in MongoDB: {ex.Message}");
            }
        }
    }
}
