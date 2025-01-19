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
    public class JobServiceTests
    {
        private readonly Mock<IJobRepository> _mockJobRepository;
        private readonly Mock<ILogger<UserRepository>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly JobService _jobService;

        public JobServiceTests()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockLogger = new Mock<ILogger<UserRepository>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "JobServiceTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _jobService = new JobService(_mockJobRepository.Object, _mockLogger.Object, _context);
        }

        private void InitializeDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateJobAsync_ReturnsJob_WhenJobIsValid()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "New Job", Assignee = 1 };
            var user = new User { Id = 1, Username = "Test User" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _mockJobRepository.Setup(repo => repo.CreateJobAsync(job)).ReturnsAsync(job);

            // Act
            var result = await _jobService.CreateJobAsync(job);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(job.Id, result.Id);
            Assert.Equal(job.Title, result.Title);
        }


        [Fact]
        public async Task GetJobByIdAsync_ReturnsJob_WhenJobExists()
        {
            // Arrange
            var job = new Job { Id = 1, Title = "Existing Job" };
            _mockJobRepository.Setup(repo => repo.GetJobByIdAsync(job.Id)).ReturnsAsync(job);

            // Act
            var result = await _jobService.GetJobByIdAsync(job.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(job.Id, result.Id);
            Assert.Equal(job.Title, result.Title);
        }

        [Fact]
        public async Task GetAllJobsAsync_ReturnsAllJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Job 1" },
                new Job { Id = 2, Title = "Job 2" }
            };
            _mockJobRepository.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobs);

            // Act
            var result = await _jobService.GetAllJobsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, job => job.Title == "Job 1");
            Assert.Contains(result, job => job.Title == "Job 2");
        }

        [Fact]
        public async Task UpdateJobAsync_ReturnsUpdatedJob_WhenJobIsValid()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "Updated Job", Assignee = 1 };
            var existingJob = new Job { Id = 1, Title = "Existing Job", Assignee = 1 };
            await _context.Jobs.AddAsync(existingJob);
            await _context.SaveChangesAsync();

            _mockJobRepository.Setup(repo => repo.UpdateJobAsync(existingJob)).ReturnsAsync(existingJob);

            // Act
            var result = await _jobService.UpdateJobAsync(job.Id, job);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingJob.Id, result.Id);
            Assert.Equal("Updated Job", result.Title);
        }


        [Fact]
        public async Task DeleteJobAsync_ReturnsTrue_WhenJobExists()
        {
            // Arrange
            InitializeDatabase();
            var job = new Job { Id = 1, Title = "Job to Delete" };
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            _mockJobRepository.Setup(repo => repo.DeleteJobAsync(job.Id)).ReturnsAsync(true);

            // Act
            var result = await _jobService.DeleteJobAsync(job.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteJobAsync_ReturnsFalse_WhenJobDoesNotExist()
        {
            // Arrange
            InitializeDatabase();
            var jobId = 1;

            // Act
            var result = await _jobService.DeleteJobAsync(jobId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetExpiredJobsAsync_ReturnsExpiredJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Expired Job 1", DueDate = DateTime.Now.AddDays(-1) },
                new Job { Id = 2, Title = "Active Job 1", DueDate = DateTime.Now.AddDays(1) }
            };
            _mockJobRepository.Setup(repo => repo.GetExpiredJobsAsync()).ReturnsAsync(jobs.Where(j => j.DueDate < DateTime.Now));

            // Act
            var result = await _jobService.GetExpiredJobsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Expired Job 1", result.First().Title);
        }

        [Fact]
        public async Task GetActiveJobsAsync_ReturnsActiveJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Expired Job 1", DueDate = DateTime.Now.AddDays(-1) },
                new Job { Id = 2, Title = "Active Job 1", DueDate = DateTime.Now.AddDays(1) }
            };
            _mockJobRepository.Setup(repo => repo.GetActiveJobsAsync()).ReturnsAsync(jobs.Where(j => j.DueDate >= DateTime.Now));

            // Act
            var result = await _jobService.GetActiveJobsAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Active Job 1", result.First().Title);
        }

        [Fact]
        public async Task GetJobsByDueDateAsync_ReturnsJobsByDueDate()
        {
            // Arrange
            var dueDate = DateTime.Now.Date;
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Job 1", DueDate = dueDate },
                new Job { Id = 2, Title = "Job 2", DueDate = dueDate.AddDays(1) }
            };
            _mockJobRepository.Setup(repo => repo.GetJobsByDueDateAsync(dueDate)).ReturnsAsync(jobs.Where(j => j.DueDate.Date == dueDate.Date));

            // Act
            var result = await _jobService.GetJobsByDueDateAsync(dueDate);

            // Assert
            Assert.Single(result);
            Assert.Equal("Job 1", result.First().Title);
        }
    }
}