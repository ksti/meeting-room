using MeetingRoom.Domain.Entities;
using MeetingRoom.Infrastructure.Data;
using MeetingRoom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MeetingRoom.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var repository = new UserRepository(context);
        var user = User.Create("testuser", "test@example.com", "Password123!");

        // Act
        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal(user.Username, savedUser.Username);
        Assert.Equal(user.Email.Value, savedUser.Email.Value);
    }
}
