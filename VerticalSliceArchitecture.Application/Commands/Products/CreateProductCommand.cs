using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.Models.Dtos;

namespace VerticalSliceArchitecture.Application.Commands.Products
{
    public class CreateProductCommand : IRequest<ProductDto>
    {

    }
}
