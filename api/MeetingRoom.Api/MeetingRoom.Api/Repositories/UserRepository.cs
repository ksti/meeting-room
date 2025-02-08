using MeetingRoom.Api.Data;
using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Data.SqlClient;

namespace MeetingRoom.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _connection;

        public UserRepository(ApplicationDbContext context, IDbConnection connection)
        {
            _context = context;
            _connection = connection;
        }
        public async Task<PagedResult<User>> SearchAsync(UserSearchRequest query)
        {
            var whereConditions = new List<string> { "IsDeleted = 0" };
            var parameters = new DynamicParameters();
            var sqlParameters = new List<SqlParameter>();
            var columnValue = new SqlParameterExpression("columnValue", "http://SomeURL");

            // Add search conditions
            if (!string.IsNullOrEmpty(query.Search))
            {
                whereConditions.Add(@"(
                    FirstName LIKE @Search OR 
                    LastName LIKE @Search OR
                    Username LIKE @Search)");
                parameters.Add("@Search", $"%{query.Search}%");
            }

            // Add status filter
            if (query.Status is not null)
            {
                whereConditions.Add("Status = @Status");
                parameters.Add("@Status", query.Status);
            }

            var whereClause = string.Join(" AND ", whereConditions);

            // Get total count
            var countSql = $@"
                SELECT COUNT(*)
                FROM Users
                WHERE {whereClause}";

            //var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, parameters);
            var totalCount = _context.Database.SqlQuery<int>($"{countSql}");

            // Set up sorting
            var orderBy = query.SortBy switch
            {
                "username" => "Username",
                "firstname" => "FirstName",
                "lastname" => "LastName",
                "status" => "Status",
                _ => "LastModifiedAt"
            };
            var sortDirection = query.SortDesc ? "DESC" : "ASC";

            // Add pagination parameters
            parameters.Add("@Offset", (query.Page - 1) * query.PageSize);
            parameters.Add("@PageSize", query.PageSize);

            // Get paginated data with explicit column selection
            var sql = $@"
                SELECT 
                    Id, FirstName, LastName, Username, Email, 
                    Contact, Avatar, Role, Status, CreatedAt, 
                    LastModifiedAt, CreatedBy, LastModifiedBy
                FROM Users
                WHERE {whereClause}
                ORDER BY {orderBy} {sortDirection}
                LIMIT @PageSize OFFSET @Offset";

            var items = await _connection.QueryAsync<User>(sql, parameters);
            string sqls = "";
            string tableName = typeof(T).Name;//获取表名
            string sql = string.Format("select *, row_number() over (order by {0} ) as row_number from {1}", string.IsNullOrEmpty(orderKey) ? "Id" : orderKey, tableName);
            string where1 = !string.IsNullOrEmpty(where) ? " where 1=1 " + where : "";
            int tag = (pageIndex - 1) * pageSize;
            sqls = string.Format(@"select top ({0}) * from 
                          ( 
                            {1}
                            {2}
                           )  as t
                          where t.row_number > {3}", pageSize, sql, where1, tag);
            //获取数据
            var list = db.Database.SqlQuery<T>(sqls, paramss).ToList<T>();

            //通过自定义的class R 取得总页码数和记录数
            string sqlCount = string.Format("select count(1) as Rows from {0} {1}", tableName, where1);
            var rows = _context.Database.SqlQuery<User>(sqlCount, paramss).ToList()[0];
            totalPage = rows % pageSize == 0 ? rows / pageSize : rows / pageSize + 1;

            return new PagedResult<User>
            {
                Data = items,
                Total = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Email != null && u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.RefreshToken == refreshToken);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.LastModifiedAt = DateTime.UtcNow;
                user.Status = UserStatus.Disabled;
                await _context.SaveChangesAsync();
            }
        }
    }
}
