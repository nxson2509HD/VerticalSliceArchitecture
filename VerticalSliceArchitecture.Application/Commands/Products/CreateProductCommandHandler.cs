using AutoMapper;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Models.Dtos;

namespace VerticalSliceArchitecture.Application.Commands.Products
{
    public class CreateProductCommandHandler
    {
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new ProductMongodb();


            // Thực hiện các thao tác lưu vào cơ sở dữ liệu ở đây

            return _mapper.Map<ProductDto>(product);
        }
    }
}
