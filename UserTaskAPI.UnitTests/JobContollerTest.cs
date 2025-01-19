using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserTasksAPI.Controllers;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Services;
using Xunit;

namespace UserTaskAPI.UnitTests.ControllerTests
{
    public class JobControllerTest
    {
        private readonly Mock<IJobService> _mockJobService;
        private readonly JobsController _controller;

        public JobControllerTest()
        {
            _mockJobService = new Mock<IJobService>();
            _controller = new JobsController(_mockJobService.Object);
        }

        [Fact]
        public async Task GetAllTasks_ReturnsOkWithJobs()
        {
            // Arrange
            var expectedJobs = new List<Job>() { new Job { Id = 1, Title = "Test Job 1" } };
            _mockJobService.Setup(service => service.GetAllJobsAsync()).ReturnsAsync(expectedJobs);

            // Act
            var result = await _controller.GetAllTasks();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(expectedJobs, okResult.Value);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest()
        {
            // Arrange
            var newJob = new Job { Id = 1, Title = "New Job" };
            _mockJobService.Setup(service => service.CreateJobAsync(newJob)).ReturnsAsync((Job)null);

            // Act
            var result = await _controller.CreateTask(newJob);

            // Assert
            var badRequestResult = result.Result as BadRequestResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNoContent()
        {
            // Arrange
            var jobId = 1;
            var updatedJob = new Job { Id = jobId, Title = "Updated Job" };
            _mockJobService.Setup(service => service.GetJobByIdAsync(jobId)).ReturnsAsync(updatedJob);
            _mockJobService.Setup(service => service.UpdateJobAsync(jobId, updatedJob)).ReturnsAsync(updatedJob);

            // Act
            var result = await _controller.UpdateTask(jobId, updatedJob);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsBadRequest()
        {
            // Arrange
            var jobId = 1;
            var updatedJob = new Job { Id = jobId, Title = "Updated Job" };
            _mockJobService.Setup(service => service.GetJobByIdAsync(jobId)).ReturnsAsync((Job)null);

            // Act
            var result = await _controller.UpdateTask(jobId, updatedJob);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task CreateTask_ReturnsCreatedAtAction()
        {
            // Arrange
            var newJob = new Job { Id = 1, Title = "New Job" };
            _mockJobService.Setup(service => service.CreateJobAsync(newJob)).ReturnsAsync(newJob);

            // Act
            var result = await _controller.CreateTask(newJob);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.Equal(nameof(_controller.GetTaskById), createdAtActionResult.ActionName);
            Assert.Equal(newJob.Id, ((Job)createdAtActionResult.Value).Id);
        }
        [Fact]
        public async Task DeleteTask_ReturnsOk()
        {
            // Arrange
            var jobId = 1;
            _mockJobService.Setup(service => service.DeleteJobAsync(jobId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTask(jobId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal("Deleted Successfully", okResult.Value);
        }
        
        [Fact]
        public async Task DeleteTask_ReturnsNotFound()
        {
            // Arrange
            var jobId = 1;
            _mockJobService.Setup(service => service.DeleteJobAsync(jobId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTask(jobId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
   public async Task GetExpiredTasks_ReturnsOkWithJobs()
        {
            // Arrange
            var expectedJobs = new List<Job>() { new Job { Id = 1, Title = "Expired Job 1" } };
            _mockJobService.Setup(service => service.GetExpiredJobsAsync()).ReturnsAsync(expectedJobs);

            // Act
            var result = await _controller.GetExpiredTasks();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(expectedJobs, okResult.Value);
        }

        [Fact]
        public async Task GetActiveTasks_ReturnsOkWithJobs()
        {
            // Arrange
            var expectedJobs = new List<Job>() { new Job { Id = 1, Title = "Active Job 1" } };
            _mockJobService.Setup(service => service.GetActiveJobsAsync()).ReturnsAsync(expectedJobs);

            // Act
            var result = await _controller.GetActiveTasks();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(expectedJobs, okResult.Value);
        }

        [Fact]
        public async Task GetTasksByDueDate_ReturnsOkWithJobs()
        {
            // Arrange
            var dueDate = DateTime.Now;
            var expectedJobs = new List<Job>() { new Job { Id = 1, Title = "Due Job 1" } };
            _mockJobService.Setup(service => service.GetJobsByDueDateAsync(dueDate)).ReturnsAsync(expectedJobs);

            // Act
            var result = await _controller.GetTasksByDueDate(dueDate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(expectedJobs, okResult.Value);
        }

        [Fact]
        public async Task GetTasksByDueDate_ReturnsBadRequest()
        {
            // Arrange
            var dueDate = DateTime.Now;
            _mockJobService.Setup(service => service.GetJobsByDueDateAsync(dueDate)).ReturnsAsync((List<Job>)null);

            // Act
            var result = await _controller.GetTasksByDueDate(dueDate);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

    }
}