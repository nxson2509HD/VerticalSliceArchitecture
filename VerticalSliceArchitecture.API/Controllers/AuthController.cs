using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VerticalSliceArchitecture.Domain.IRepositories;

namespace VerticalSliceArchitecture.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IMediator mediator, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _userRepository = userRepository;
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                BadRequest(response);
            }

            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(RegisterCommand command, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AddUserToRole(AssignRoleCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return response.Data == null ? NotFound(response) : BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RevokeRole")]
        public async Task<IActionResult> RemoveUserFromRole(RevokeRoleCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return response.Data == null ? NotFound(response) : BadRequest(response);
            }

            return Ok(response);
        }
    }
}
