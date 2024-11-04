using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.IRepositories;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Models.Dtos;
using VerticalSliceArchitecture.Domain.Services;

namespace VerticalSliceArchitecture.Application.Queries.Products.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, StandardResponse?>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IRedisCacheService _redis;



        public GetProductByIdQueryHandler(IMapper mapper, ILogger<GetProductByIdQueryHandler> logger,
              IProductRepository productRepository, IRedisCacheService redis)
        {
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepository;
            _redis = redis;
        }

        public async Task<StandardResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Logic để lấy thực thể từ cơ sở dữ liệu
            string cacheKey = $"Product:{request.Id}";

            // lưu log
            _logger.LogInformation($"Getting product by ID {request.Id}");
            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product != null)
            {
                //lưu redis
                await _redis.SetCacheAsync(cacheKey, product, TimeSpan.FromHours(1));
                return new StandardResponse
                {
                    Success = true,
                    Data = _mapper.Map<ProductDto?>(product),
                    Message = "Action Successfully!"
                };
            }
            else
            {
                return new StandardResponse
                {
                    Success = false,
                    Message = $"Product with ID {request.Id} was not found"
                };
            }
        }
    }
}
