using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Application.Features.Users.Commands.RegisterUser;
using MeetingRoom.Application.Features.Users.Commands.LoginUser;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Interfaces;

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<string>>> Register([FromBody] RegisterUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<LoginResponseDto>>> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<Result<bool>>> Logout()
        {
            // 在整洁架构中，我们使用命令来处理注销操作
            // 这里我们需要创建一个LogoutUserCommand
            var command = new LogoutUserCommand
            {
                UserId = _currentUserService.UserId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
