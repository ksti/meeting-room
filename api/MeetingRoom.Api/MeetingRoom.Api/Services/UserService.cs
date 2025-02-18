using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Extensions;
using MeetingRoom.Api.Models;
using MeetingRoom.Api.Repositories;

namespace MeetingRoom.Api.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<UserModel> CreateUserAsync(UserRegisterRequest request, string operatorId)
        {
            var existedUser = await userRepository.GetByEmailAsync(request.Email);
            if (existedUser != null) throw new BusinessException("User already existed");

            var newUser = new UserEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
                Role = request.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

            };
            newUser.SetCreated(operatorId);
            await userRepository.CreateAsync(newUser);
            return newUser.MapToModel();
        }

        public async Task DeleteUserAsync(string id)
        {
            await userRepository.DeleteAsync(id);
        }

        public async Task<UserModel?> GetUserByIdAsync(string id)
        {
            var userEntity = await userRepository.GetByIdAsync(id);
            return userEntity?.MapToModel();
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            var userEntity = await userRepository.GetByEmailAsync(email);
            return userEntity?.MapToModel();
        }

        public async Task<PagedResult<UserModel>> SearchAsync(UserSearchRequest request)
        {
            var result = await userRepository.SearchAsync(request);
            return new PagedResult<UserModel>
            {
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total,
                Data = result.Data.Select(u => u.MapToModel())
            };
        }

        public async Task<UserModel> UpdateUserAsync(UserRegisterRequest request, string operatorId)
        {
            return await UpdateUserPartialAsync(request, operatorId);
        }

        public async Task<UserModel> UpdateUserPartialAsync(UserUpdateRequest request, string operatorId)
        {
            var existedUser = await userRepository.GetByEmailAsync(request.Email);
            if (existedUser == null) throw new NotFoundException();

            if (!string.IsNullOrEmpty(request.FirstName))
                existedUser.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                existedUser.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Email))
                existedUser.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Role))
                existedUser.Role = request.Role;
            if (!string.IsNullOrEmpty(request.Username))
                existedUser.Username = request.Username;
            if (!string.IsNullOrEmpty(request.Password))
                existedUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            existedUser.SetModified(operatorId);
            await userRepository.UpdateAsync(existedUser);
            return existedUser.MapToModel();
        }
    }
}
