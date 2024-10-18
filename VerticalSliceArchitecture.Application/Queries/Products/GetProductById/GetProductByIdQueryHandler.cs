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

namespace VerticalSliceArchitecture.Application.Queries.Products.GetProductById
{
    public class GetProductByIdQueryHandler :  IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;
        private readonly IProductRepository _productRepository;


        public GetProductByIdQueryHandler(IMapper mapper, ILogger<GetProductByIdQueryHandler> logger,
              IProductRepository productRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // Logic để lấy thực thể từ cơ sở dữ liệu
            _logger.LogInformation("Getting all products");
            var products = await _productRepository.GetByIdAsync(request.Id);

            return _mapper.Map<ProductDto?>(products); ;
        }
    }
}
