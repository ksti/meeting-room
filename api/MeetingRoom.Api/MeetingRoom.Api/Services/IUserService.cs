using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserModel>> SearchAsync(UserSearchRequest request);
        Task<UserModel?> GetUserByIdAsync(int id);
        Task<UserModel> CreateUserAsync(UserModel userModel, int operatorId);
        Task<UserModel> UpdateUserAsync(UserModel request, int operatorId);
        Task DeleteUserAsync(int id);
    }
}
