using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Application.Features.Rooms.Commands.CreateRoom;
using MeetingRoom.Application.Features.Rooms.Queries.GetRoomById;
using MeetingRoom.Application.Features.Rooms.Queries.GetRoomsList;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Interfaces;

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<RoomsController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedList<RoomDto>>>> GetRooms(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchKeyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int? minCapacity = null,
            [FromQuery] int? maxCapacity = null,
            [FromQuery] bool? onlyAvailable = null)
        {
            var query = new GetRoomsListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchKeyword = searchKeyword,
                Status = status,
                MinCapacity = minCapacity,
                MaxCapacity = maxCapacity,
                OnlyAvailable = onlyAvailable
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<RoomDto>>> GetRoom(string id)
        {
            var query = new GetRoomByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<string>>> CreateRoom([FromBody] CreateRoomCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetRoom), new { id = result.Data }, result);
        }
    }
}
