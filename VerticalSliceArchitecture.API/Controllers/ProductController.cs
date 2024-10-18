using MediatR;
using Microsoft.AspNetCore.Mvc;
using VerticalSliceArchitecture.Application.Commands.Products;
using VerticalSliceArchitecture.Application.Queries.Products.GetProductById;
using VerticalSliceArchitecture.Domain.Models.Dtos;
using VerticalSliceArchitecture.Domain.Models;
using Newtonsoft.Json;
using Asp.Versioning;

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

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        //}
        [ApiVersion("1.0")]
        [HttpGet("GetByIdv1/{id}")]


        public async Task<IActionResult> GetById(int id)
        {
            var response = new StandardResponse<ProductDto?>();

            try
            {
                var query = new GetProductByIdQuery(id);
                var result = await _mediator.Send(query);

                response.Success = true;
                response.Data = result;
                response.Message = "Products retrieved successfully - V1";

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));

                response.Success = false;
                response.Message = "An unexpected error occurred";
                response.ExceptionMessage = ex.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
        [ApiVersion("2.0")]

        [HttpGet("GetByIdv2/{id}")]

        public async Task<IActionResult> GetByIdv2(int id)
        {
            var response = new StandardResponse<ProductDto?>();

            try
            {
                var query = new GetProductByIdQuery(id);
                var result = await _mediator.Send(query);

                response.Success = true;
                response.Data = result;
                response.Message = "Products retrieved successfully - V2";

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));

                response.Success = false;
                response.Message = "An unexpected error occurred";
                response.ExceptionMessage = ex.Message;

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
