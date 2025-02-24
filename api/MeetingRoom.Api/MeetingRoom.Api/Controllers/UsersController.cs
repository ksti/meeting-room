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
    public class UsersController(ILogger<UsersController> logger, IUserService userService, ICurrentUserService currentUserService) : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ApiResponse<PagedResult<UserModel>>> Get(UserSearchRequest request)
        {
            var result = await userService.SearchAsync(request);
            return ApiResponse<PagedResult<UserModel>>.Ok(result);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ApiResponse<UserModel?>> Get(string id)
        {
            var result = await userService.GetUserByIdAsync(id);
            return ApiResponse<UserModel?>.Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ApiResponse<UserModel>> Post([FromBody] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse<UserModel>.BadRequest(ModelState);
            }
            var currentUser = await currentUserService.GetCurrentUserAsync();
            var result = await userService.CreateUserAsync(request, currentUser!.Id);
            return ApiResponse<UserModel>.Ok(result);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> Put(string id, [FromBody] UserUpdateRequest request)
        {
            request.Id = id;
            var currentUser = await currentUserService.GetCurrentUserAsync();
            await userService.UpdateUserPartialAsync(request, currentUser!.Id);
            return ApiResponse<bool>.Ok(true);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            await userService.DeleteUserAsync(id);
        }
    }
}
