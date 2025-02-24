using MeetingRoom.Api.Data;
using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace MeetingRoom.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<UserEntity>> SearchAsync(UserSearchRequest searchRequest)
        {
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new List<SqlParameter>();

            // Add search conditions
            if (!string.IsNullOrEmpty(searchRequest.Search))
            {
                whereConditions.Add(@"(
                    FirstName LIKE @Search OR 
                    LastName LIKE @Search OR
                    Username LIKE @Search)");
                parameters.Add(new SqlParameter("@Search", $"%{searchRequest.Search}%"));
            }

            // Add status filter
            if (searchRequest.Status is not null)
            {
                whereConditions.Add("Status = @Status");
                parameters.Add(new SqlParameter("@Status", searchRequest.Status));
            }

            var whereClause = string.Join(" AND ", whereConditions);

            // Get total count
            var countSql = $@"
                SELECT COUNT(*)
                FROM Users
                WHERE {whereClause}";

            //var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, parameters);
            var totalCount = _context.Database.SqlQueryRaw<int>(countSql, parameters);

            // Set up sorting
            var orderBy = searchRequest.SortBy switch
            {
                "username" => "Username",
                "firstname" => "FirstName",
                "lastname" => "LastName",
                "status" => "Status",
                _ => "UpdatedAt"
            };
            var sortDirection = searchRequest.SortDesc ? "DESC" : "ASC";

            // Add pagination parameters
            parameters.Add(new SqlParameter("@Offset", (searchRequest.Page - 1) * searchRequest.PageSize));
            parameters.Add(new SqlParameter("@PageSize", searchRequest.PageSize));

            // Get paginated data with explicit column selection
            var sql = $@"
                SELECT 
                    Id, FirstName, LastName, Username, Email, 
                    Contact, Avatar, Role, Status, CreatedAt, 
                    UpdatedAt, CreatedBy, UpdatedBy
                FROM Users
                WHERE {whereClause}
                ORDER BY {orderBy} {sortDirection}
                LIMIT @PageSize OFFSET @Offset";

            var items = _context.Users.FromSqlRaw(sql, parameters);

            return new PagedResult<UserEntity>
            {
                Data = items,
                Total = totalCount.FirstOrDefault(),
                Page = searchRequest.Page,
                PageSize = searchRequest.PageSize
            };
        }

        public async Task<UserEntity?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Username.ToLower() == username.ToLower());
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Email.ToLower() == email.ToLower());
        }

        public async Task<UserEntity?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.Tokens)
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Tokens.Any(t => t.RefreshToken == refreshToken));
        }

        public async Task<UserEntity> CreateAsync(UserEntity userEntity)
        {
            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();
            return userEntity;
        }

        public async Task UpdateAsync(UserEntity userEntity)
        {
            _context.Entry(userEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
