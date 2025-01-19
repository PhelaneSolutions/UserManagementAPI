using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UserTasksAPI.Data;
using UserTasksAPI.Models;
using UserTasksAPI.Repositories;
using Xunit;

namespace UserTaskAPI.UnitTests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<ILogger<UserRepository>> _mockLogger;
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _context;
        private UserRepository _repository;

        public UserRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<UserRepository>>();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UserRepositoryTests")
                .Options;
        }

        private void InitializeDatabase()
        {
            _context = new ApplicationDbContext(_options);
            _context.Database.EnsureDeleted(); 
            _context.Database.EnsureCreated(); 
            _repository = new UserRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "Test User" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            InitializeDatabase();

            // Act
            var result = await _repository.GetUserByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            InitializeDatabase();
            var users = new List<User>
            {
                new User { Id = 1, Username = "User 1" },
                new User { Id = 2, Username = "User 2" }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, user => user.Username == "User 1");
            Assert.Contains(result, user => user.Username == "User 2");
        }

        [Fact]
        public async Task CreateUserAsync_AddsUserToDatabase()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "New User" };

            // Act
            var result = await _repository.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUserInDatabase()
        {
            // Arrange
            InitializeDatabase();
            var user = new User { Id = 1, Username = "New User" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            user.Username = "Updated User";

            // Act
            var result = await _repository.UpdateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal("Updated User", result.Username);
        }
    }
}