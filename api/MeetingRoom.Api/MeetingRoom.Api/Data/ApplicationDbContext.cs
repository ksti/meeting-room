using MeetingRoom.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Data
{
    public class ApplicationDbContext(IConfiguration configuration) : IdentityDbContext<IdentityUser>
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            optionsBuilder.UseMySql(ServerVersion.AutoDetect(connectionString));
        }
    }
}
