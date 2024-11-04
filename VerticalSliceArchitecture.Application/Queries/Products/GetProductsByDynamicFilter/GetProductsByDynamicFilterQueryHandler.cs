using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VerticalSliceArchitecture.Application.Queries.Products.GetProductById;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Models.Dtos;

namespace VerticalSliceArchitecture.Application.Queries.Products.GetProductsByDynamicFilter
{
    public class GetProductsByDynamicFilterQueryHandler :  IRequestHandler<GetProductsByDynamicFilterQuery, StandardResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductsByDynamicFilterQueryHandler> _logger;
        private readonly IProductRepository _productRepository;

        public GetProductsByDynamicFilterQueryHandler(IMapper mapper, ILogger<GetProductsByDynamicFilterQueryHandler> logger,
              IProductRepository productRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<StandardResponse> Handle(GetProductsByDynamicFilterQuery request, CancellationToken cancellationToken)
        {
            // Logic để lấy thực thể từ cơ sở dữ liệu
            _logger.LogInformation("Getting all products");
            var products = await _productRepository.GetProductsDynamicFilter(request.Filter);

            return  new StandardResponse
            {
                Success = true,
                Data = _mapper.Map<ProductDto?>(products),
                Message = "Product retrieved successfully"
            };

        }
    }
}
