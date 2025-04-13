using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Application.Features.Meetings.Commands.CreateMeeting;
using MeetingRoom.Application.Features.Meetings.Queries.GetMeetingById;
using MeetingRoom.Application.Features.Meetings.Queries.GetMeetingsList;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Application.Interfaces;

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<MeetingsController> _logger;

        public MeetingsController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<MeetingsController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedList<MeetingDto>>>> GetMeetings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchKeyword = null,
            [FromQuery] string? roomId = null,
            [FromQuery] string? organizerId = null,
            [FromQuery] string? participantId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null)
        {
            var query = new GetMeetingsListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchKeyword = searchKeyword,
                RoomId = roomId,
                OrganizerId = organizerId,
                ParticipantId = participantId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<MeetingDetailDto>>> GetMeeting(string id)
        {
            var query = new GetMeetingByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Result<string>>> CreateMeeting([FromBody] CreateMeetingCommand command)
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

            return CreatedAtAction(nameof(GetMeeting), new { id = result.Data }, result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<Result<PaginatedList<MeetingDto>>>> GetMyMeetings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(Result<PaginatedList<MeetingDto>>.Failure("未授权"));
            }

            var query = new GetMeetingsListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                ParticipantId = userId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("organized")]
        public async Task<ActionResult<Result<PaginatedList<MeetingDto>>>> GetOrganizedMeetings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(Result<PaginatedList<MeetingDto>>.Failure("未授权"));
            }

            var query = new GetMeetingsListQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrganizerId = userId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
