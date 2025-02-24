using MeetingRoom.Api.Data;
using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<RoomEntity>> SearchAsync(RoomSearchRequest searchRequest)
        {
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new List<SqlParameter>();

            // Add search conditions
            if (!string.IsNullOrEmpty(searchRequest.Search))
            {
                whereConditions.Add(@"(
                    Name LIKE @Search)");
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
                FROM Rooms
                WHERE {whereClause}";

            var totalCount = _context.Database.SqlQueryRaw<int>(countSql, parameters);

            // Set up sorting
            var orderBy = searchRequest.SortBy switch
            {
                "name" => "Name",
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
                    Id, Name, Description, Capacity, Status, CreatedAt, 
                    UpdatedAt, CreatedBy, UpdatedBy
                FROM Rooms
                WHERE {whereClause}
                ORDER BY {orderBy} {sortDirection}
                LIMIT @PageSize OFFSET @Offset";

            var items = _context.Rooms.FromSqlRaw(sql, parameters);

            return new PagedResult<RoomEntity>
            {
                Data = items,
                Total = totalCount.FirstOrDefault(),
                Page = searchRequest.Page,
                PageSize = searchRequest.PageSize
            };
        }

        public async Task<RoomEntity?> GetByIdAsync(string id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<RoomEntity> CreateAsync(RoomEntity roomEntity)
        {
            _context.Rooms.Add(roomEntity);
            await _context.SaveChangesAsync();
            return roomEntity;
        }

        public async Task UpdateAsync(RoomEntity roomEntity)
        {
            _context.Entry(roomEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var deviceEntity = await _context.Devices.FindAsync(id);
            if (deviceEntity != null)
            {
                _context.Devices.Remove(deviceEntity);
            }
        }
    }
}
