using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserModel>> SearchAsync(UserSearchRequest request);
        Task<UserModel?> GetUserByIdAsync(string id);
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<UserModel> CreateUserAsync(UserRegisterRequest request, string operatorId);
        Task<UserModel> UpdateUserAsync(UserRegisterRequest request, string operatorId);
        Task<UserModel> UpdateUserPartialAsync(UserUpdateRequest request, string operatorId);
        Task DeleteUserAsync(string id);
    }
}
