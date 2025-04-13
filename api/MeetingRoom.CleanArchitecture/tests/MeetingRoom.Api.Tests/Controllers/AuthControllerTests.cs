using MediatR;
using MeetingRoom.Api.Controllers;
using MeetingRoom.Application.Commands.Users;
using MeetingRoom.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MeetingRoom.Api.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("userId"));

        var controller = new AuthController(mediatorMock.Object);
        var command = new RegisterUserCommand
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Register(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Result<string>>(okResult.Value);
        Assert.True(returnValue.Succeeded);
        Assert.Equal("userId", returnValue.Data);
    }
}
