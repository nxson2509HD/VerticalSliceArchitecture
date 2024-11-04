using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerticalSliceArchitecture.Domain.Models;
using VerticalSliceArchitecture.Domain.Models.Dtos;

namespace VerticalSliceArchitecture.Application.Queries.Products.GetProductsByDynamicFilter
{
    public class GetProductsByDynamicFilterQuery : IRequest<StandardResponse?>
    {
        public string Filter { get; set; }

        public GetProductsByDynamicFilterQuery(string filter)
        {
            Filter = filter;
        }
    }
}
