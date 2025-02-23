using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Extensions;
using MeetingRoom.Api.Models;
using MeetingRoom.Api.Repositories;

namespace MeetingRoom.Api.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<UserModel> CreateUserAsync(UserCreateRequest request, string operatorId)
        {
            var old = await userRepository.GetByEmailAsync(request.Email);
            if (old != null) throw new BusinessException("User already existed");

            var newEntity = new UserEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.Username,
                Role = request.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

            };
            newEntity.SetCreated(operatorId);
            await userRepository.CreateAsync(newEntity);
            return newEntity.MapToModel();
        }

        public async Task DeleteUserAsync(string id)
        {
            await userRepository.DeleteAsync(id);
        }

        public async Task<UserModel?> GetUserByIdAsync(string id)
        {
            var entity = await userRepository.GetByIdAsync(id);
            return entity?.MapToModel();
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            var entity = await userRepository.GetByEmailAsync(email);
            return entity?.MapToModel();
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

        public async Task<UserModel> UpdateUserAsync(UserCreateRequest request, string operatorId)
        {
            return await UpdateUserPartialAsync(request, operatorId);
        }

        public async Task<UserModel> UpdateUserPartialAsync(UserUpdateRequest request, string operatorId)
        {
            var old = await userRepository.GetByIdAsync(request.Id);
            if (old == null) throw new NotFoundException();

            if (!string.IsNullOrEmpty(request.FirstName))
                old.FirstName = request.FirstName;
            if (!string.IsNullOrEmpty(request.LastName))
                old.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.Email))
                old.Email = request.Email;
            if (!string.IsNullOrEmpty(request.Role))
                old.Role = request.Role;
            if (!string.IsNullOrEmpty(request.Username))
                old.Username = request.Username;
            if (!string.IsNullOrEmpty(request.Password))
                old.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            old.SetModified(operatorId);
            await userRepository.UpdateAsync(old);
            return old.MapToModel();
        }
    }
}
