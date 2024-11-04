using MediatR;
using Microsoft.AspNetCore.Mvc;
using VerticalSliceArchitecture.Application.Commands.Products;
using VerticalSliceArchitecture.Application.Queries.Products.GetProductById;
using VerticalSliceArchitecture.Domain.Models.Dtos;
using VerticalSliceArchitecture.Domain.Models;
using Newtonsoft.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using VerticalSliceArchitecture.Application.Queries.Products.GetProductsByDynamicFilter;

namespace VerticalSliceArchitecture.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [ApiVersion("1.0")]
        [HttpGet("GetByIdv1/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            result.Message = "Products retrieved successfully - V1";
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [ApiVersion("2.0")]

        [HttpGet("GetByIdv2/{id}")]

        public async Task<IActionResult> GetByIdv2(int id)
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            result.Message = "Products retrieved successfully - V2";
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> GetProductsWithFilter(string filterExpression)
        {
            var query = new GetProductsByDynamicFilterQuery(filterExpression);
            var result = await _mediator.Send(query);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
