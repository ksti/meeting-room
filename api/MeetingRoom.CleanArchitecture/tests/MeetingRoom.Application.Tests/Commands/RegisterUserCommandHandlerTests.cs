using MeetingRoom.Application.Features.Users.Commands.RegisterUser;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using MeetingRoom.Infrastructure.Services;

namespace MeetingRoom.Application.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenCommandIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RegisterUserCommandHandler(userRepositoryMock.Object, new DateTimeService());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
