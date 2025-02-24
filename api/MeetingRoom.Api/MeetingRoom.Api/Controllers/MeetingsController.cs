using MeetingRoom.Api.Models;
using MeetingRoom.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetingsController(ILogger<MeetingsController> logger, IMeetingService meetingService, ICurrentUserService currentUserService) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ApiResponse<PagedResult<MeetingModel>>> Get(MeetingSearchRequest request)
        {
            var result = await meetingService.SearchAsync(request);
            return ApiResponse<PagedResult<MeetingModel>>.Ok(result);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ApiResponse<MeetingModel?>> Get(string id)
        {
            var result = await meetingService.GetMeetingByIdAsync(id);
            return ApiResponse<MeetingModel?>.Ok(result);
        }

        // GET api/<UsersController>/userId/5
        [HttpGet("userId/{userId}")]
        public async Task<ApiResponse<ICollection<MeetingModel>>> GetMeetingsByUserId(string userId, DateTime startTime, DateTime endTime)
        {
            var result = await meetingService.GetMeetingsByUserIdAsync(userId, startTime, endTime);
            return ApiResponse<ICollection<MeetingModel>>.Ok(result);
        }

        // GET api/<UsersController>/roomId/5
        [HttpGet("roomId/{roomId}")]
        public async Task<ApiResponse<ICollection<MeetingModel>>> GetMeetingsByRoomId(string roomId, DateTime startTime, DateTime endTime)
        {
            var result = await meetingService.GetMeetingsByRoomIdAsync(roomId, startTime, endTime);
            return ApiResponse<ICollection<MeetingModel>>.Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ApiResponse<MeetingModel>> Post([FromBody] MeetingCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse<MeetingModel>.BadRequest(ModelState);
            }
            var currentUser = await currentUserService.GetCurrentUserAsync();
            var result = await meetingService.UpdateMeetingAsync(request, currentUser!.Id);
            return ApiResponse<MeetingModel>.Ok(result);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> Put(string id, [FromBody] MeetingUpdateRequest request)
        {
            var currentUser = await currentUserService.GetCurrentUserAsync();
            await meetingService.UpdateMeetingPartialAsync(request, currentUser!.Id);
            return ApiResponse<bool>.Ok(true);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            await meetingService.DeleteMeetingAsync(id);
        }
    }
}
