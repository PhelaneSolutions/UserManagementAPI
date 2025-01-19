using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserTasksAPI.Controllers;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Repositories;
using UserTasksAPI.Services;
using Xunit;

namespace UserTaskAPI.UnitTests.ControllerTests
{
    public class UsersControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new UsersController(_mockUserService.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUserById_ReturnsOkWithUser()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User { Id = userId, Username = "Test User" };
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(expectedUser, okResult.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAtAction()
        {
            // Arrange
            var newUser = new User { Id = 1, Username = "New User" };
            _mockUserService.Setup(service => service.CreateUserAsync(newUser)).ReturnsAsync(newUser);

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.Equal(nameof(_controller.GetUser), createdAtActionResult.ActionName);
            Assert.Equal(newUser.Id, ((User)createdAtActionResult.Value).Id);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            var updatedUser = new User { Id = userId, Username = "Updated User" };
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.UpdateUser(userId, updatedUser);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk()
        {
            var userId = 1;
            _mockUserService.Setup(service => service.DeleteUserAsync(userId)).ReturnsAsync(true);
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(new User { Id = userId });

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
          
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(service => service.DeleteUserAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}