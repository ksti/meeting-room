using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.ValueObjects;
using Xunit;

namespace MeetingRoom.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateValidUser()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "Password123!";

        // Act
        var user = User.Create(username, email, password);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email.Value);
        Assert.True(user.Password.Verify(password));
    }
}
