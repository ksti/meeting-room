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
    public class RoomsController(ILogger<RoomsController> logger, IRoomService roomService, ICurrentUserService currentUserService) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ApiResponse<PagedResult<RoomModel>>> Get(RoomSearchRequest request)
        {
            var result = await roomService.SearchAsync(request);
            return ApiResponse<PagedResult<RoomModel>>.Ok(result);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ApiResponse<RoomModel?>> Get(string id)
        {
            var result = await roomService.GetRoomByIdAsync(id);
            return ApiResponse<RoomModel?>.Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ApiResponse<RoomModel>> Post([FromBody] RoomCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse<RoomModel>.BadRequest(ModelState);
            }
            var currentUser = await currentUserService.GetCurrentUserAsync();
            var result = await roomService.CreateRoomAsync(request, currentUser!.Id);
            return ApiResponse<RoomModel>.Ok(result);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> Put(string id, [FromBody] RoomUpdateRequest request)
        {
            var currentUser = await currentUserService.GetCurrentUserAsync();
            await roomService.UpdateRoomPartialAsync(request, currentUser!.Id);
            return ApiResponse<bool>.Ok(true);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            await roomService.DeleteRoomAsync(id);
        }
    }
}
