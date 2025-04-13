using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Application.Features.Users.Queries.GetUserById;
using MeetingRoom.Application.Features.Users.Queries.GetUsersList;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Interfaces;

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedList<UserDto>>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchKeyword = null,
            [FromQuery] string? role = null,
            [FromQuery] string? status = null)
        {
            var query = new GetUsersListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchKeyword = searchKeyword,
                Role = role,
                Status = status
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<UserDto>>> GetUser(string id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<ActionResult<Result<UserDto>>> GetCurrentUser()
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(Result<UserDto>.Failure("未授权"));
            }

            var query = new GetUserByIdQuery { Id = userId };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
