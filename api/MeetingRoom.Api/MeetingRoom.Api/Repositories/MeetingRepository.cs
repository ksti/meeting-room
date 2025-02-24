using MeetingRoom.Api.Data;
using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDbContext _context;

        public MeetingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<MeetingEntity>> SearchAsync(MeetingSearchRequest searchRequest)
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
                FROM Meetings
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
                    Id, Name, Description, Capacity, StartTime, EndTime, RoomId, Status, CreatedAt, 
                    UpdatedAt, CreatedBy, UpdatedBy
                FROM Meetings
                WHERE {whereClause}
                ORDER BY {orderBy} {sortDirection}
                LIMIT @PageSize OFFSET @Offset";

            var items = _context.Meetings.FromSqlRaw(sql, parameters);

            return new PagedResult<MeetingEntity>
            {
                Data = items,
                Total = totalCount.FirstOrDefault(),
                Page = searchRequest.Page,
                PageSize = searchRequest.PageSize
            };
        }

        public async Task<MeetingEntity?> GetByIdAsync(string id)
        {
            return await _context.Meetings.FindAsync(id);
        }

        public async Task<ICollection<MeetingEntity>> GetMeetingsByTimeAsync(DateTime startTime, DateTime endTime)
        {
            var items = _context.Meetings
                .Where(m => m.StartTime >= startTime && m.EndTime <= endTime);

            return items.ToList();
        }

        public async Task<ICollection<MeetingEntity>> GetMeetingsByUserIdAsync(string userId, DateTime startTime, DateTime endTime)
        {
            var items = _context.Meetings
                .Where(m => m.StartTime >= startTime 
                            && m.EndTime <= endTime
                            && m.Attendees.Any(u => u.Id == userId))
                .Include(m => m.Attendees);

            return items.ToList();
        }

        public async Task<ICollection<MeetingEntity>> GetMeetingsByRoomIdAsync(string roomId, DateTime startTime, DateTime endTime)
        {
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new List<SqlParameter>();

            // Add search conditions
            whereConditions.Add("RoomId=@RoomId AND StartTime>=@StartTime AND EndTime<=@EndTime");
            parameters.Add(new SqlParameter("@RoomId", roomId));
            parameters.Add(new SqlParameter("@StartTime", startTime));
            parameters.Add(new SqlParameter("@EndTime", endTime));

            var whereClause = string.Join(" AND ", whereConditions);

            // Get paginated data with explicit column selection
            var sql = $@"
                SELECT 
                    Id, Name, Description, Capacity, StartTime, EndTime, RoomId, Status, CreatedAt, 
                    UpdatedAt, CreatedBy, UpdatedBy
                FROM Meetings
                WHERE {whereClause}";

            var items = _context.Meetings.FromSqlRaw(sql, parameters);

            return items.ToList();
        }

        public async Task<MeetingEntity> CreateAsync(MeetingEntity entity)
        {
            _context.Meetings.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(MeetingEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
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
