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
    public class JobRepositoryTests
    {
        private readonly Mock<ILogger<JobRepository>> _mockLogger;
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _context;
        private JobRepository _repository;

        public JobRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<JobRepository>>();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "JobRepositoryTests")
                .Options;
        }

        private void InitializeDatabase()
        {
            _context = new ApplicationDbContext(_options);
            _context.Database.EnsureDeleted(); 
            _context.Database.EnsureCreated(); 
            _repository = new JobRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllJobsAsync_ReturnsAllJobs()
        {
            // Arrange
            InitializeDatabase();
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Job 1" },
                new Job { Id = 2, Title = "Job 2" }
            };

            await _context.Jobs.AddRangeAsync(jobs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllJobsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, job => job.Title == "Job 1");
            Assert.Contains(result, job => job.Title == "Job 2");
        }

        [Fact]
        public async Task GetJobByIdAsync_ReturnsJob()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "Job 1" };
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetJobByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Job 1", result.Title);
        }

        [Fact]
        public async Task GetJobByIdAsync_ReturnsNull_WhenJobNotFound()
        {
            // Arrange
            InitializeDatabase();

            // Act
            var result = await _repository.GetJobByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateJobAsync_AddsJobToDatabase()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "New Job" };

            // Act
            var result = await _repository.CreateJobAsync(job);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(job.Id, result.Id);
            Assert.Equal(job.Title, result.Title);
        }

        [Fact]
        public async Task UpdateJobAsync_UpdatesJobInDatabase()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "New Job" };
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            job.Title = "Updated Job";

            // Act
            var result = await _repository.UpdateJobAsync(job);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(job.Id, result.Id);
            Assert.Equal("Updated Job", result.Title);
        }

        [Fact]
        public async Task DeleteJobAsync_RemovesJobFromDatabase()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "New Job" };
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteJobAsync(job.Id);

            // Assert
            Assert.True(result);
            var deletedJob = await _context.Jobs.FindAsync(job.Id);
            Assert.Null(deletedJob);
        }

        [Fact]
        public async Task GetExpiredJobsAsync_ReturnsExpiredJobs()
        {
            // Arrange
            InitializeDatabase();
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Expired Job 1", DueDate = DateTime.Now.AddDays(-1) },
                new Job { Id = 2, Title = "Active Job 1", DueDate = DateTime.Now.AddDays(1) }
            };

            await _context.Jobs.AddRangeAsync(jobs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetExpiredJobsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Expired Job 1", result.First().Title);
        }

        [Fact]
        public async Task GetActiveJobsAsync_ReturnsActiveJobs()
        {
            // Arrange
            InitializeDatabase();
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Expired Job 1", DueDate = DateTime.Now.AddDays(-1) },
                new Job { Id = 2, Title = "Active Job 1", DueDate = DateTime.Now.AddDays(1) }
            };

            await _context.Jobs.AddRangeAsync(jobs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveJobsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Active Job 1", result.First().Title);
        }

        [Fact]
        public async Task GetJobsByDueDateAsync_ReturnsJobsByDueDate()
        {
            // Arrange
            InitializeDatabase();
            var dueDate = DateTime.Now.Date;
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Job 1", DueDate = dueDate },
                new Job { Id = 2, Title = "Job 2", DueDate = dueDate.AddDays(1) }
            };

            await _context.Jobs.AddRangeAsync(jobs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetJobsByDueDateAsync(dueDate);

            // Assert
            Assert.Single(result);
            Assert.Equal("Job 1", result.First().Title);
        }
    }
}