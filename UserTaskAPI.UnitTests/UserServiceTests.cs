using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UserTasksAPI.Data;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Repositories;
using UserTasksAPI.Services;
using Xunit;

namespace UserTaskAPI.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UserServiceTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object, _context);
        }

        private void InitializeDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsUser_WhenUserIsValid()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "New User", Email = "newuser@example.com", Password = "password" };
            _mockUserRepository.Setup(repo => repo.CreateUserAsync(user)).ReturnsAsync(user);

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "Existing User" };
            _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "User 1" },
                new User { Id = 2, Username = "User 2" }
            };
            _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, user => user.Username == "User 1");
            Assert.Contains(result, user => user.Username == "User 2");
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsUpdatedUser_WhenUserIsValid()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "Updated User", Email = "updateduser@example.com", Password = "newpassword" };
            var existingUser = new User { Id = 1, Username = "Existing User", Email = "existinguser@example.com", Password = "password" };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            _mockUserRepository.Setup(repo => repo.UpdateUserAsync(existingUser)).ReturnsAsync(existingUser);

            // Act
            var result = await _userService.UpdateUserAsync(user.Id, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal("Updated User", result.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "Updated User", Email = "updateduser@example.com", Password = "newpassword" };

            // Act
            var result = await _userService.UpdateUserAsync(user.Id, user);

            // Assert
            Assert.Null(result);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"User with ID {user.Id} not found.")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "User to Delete" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _mockUserRepository.Setup(repo => repo.DeleteUserAsync(user.Id)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUserAsync(user.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            InitializeDatabase();
            var userId = 1;

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result);
        }
    }
}