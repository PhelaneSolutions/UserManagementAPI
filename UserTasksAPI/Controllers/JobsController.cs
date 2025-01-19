using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Services;

namespace UserTasksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService taskService)
        {
            _jobService = taskService;
        }
        
        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>A list of jobs.</returns>
        [HttpGet, Route("all")]
        public async Task<ActionResult<IEnumerable<Job>>> GetAllTasks()
        {
            var tasks = await _jobService.GetAllJobsAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Gets a job by ID.
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <returns>The job with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetTaskById(int id)
        {
            var task = await _jobService.GetJobByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        /// <summary>
        /// Creates a new job.
        /// </summary>
        /// <param name="job">The job to create.</param>
        /// <returns>The created job.</returns>
        [HttpPost]
        public async Task<ActionResult<Job>> CreateTask(Job job)
        {
            var createdTask = await _jobService.CreateJobAsync(job);
            if(createdTask != null)
            {
                return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
            }
            return BadRequest();
        }

        /// <summary>
        /// Updates an existing job.
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <param name="job">The updated job.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, Job job)
        {
            var existingJob = await _jobService.GetJobByIdAsync(id);
            if(existingJob == null)
            {
                return BadRequest();
            }

            await _jobService.UpdateJobAsync(id,job);
            return NoContent();
        }

        /// <summary>
        /// Deletes a job by ID.
        /// </summary>
        /// <param name="id">The job ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
           var job = await _jobService.DeleteJobAsync(id);

           if(job != true)
           {
                return NotFound();
           }
            return Ok("Deleted Successfully");
        }

        /// <summary>
        /// Gets all expired jobs.
        /// </summary>
        /// <returns>A list of jobs.</returns>
        [HttpGet("expired")]
        public async Task<ActionResult<IEnumerable<Job>>> GetExpiredTasks()
        {
            var jobs = await _jobService.GetExpiredJobsAsync();
            return Ok(jobs);
        }

         /// <summary>
        /// Gets all active jobs.
        /// </summary>
        /// <returns>A list of jobs.</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Job>>> GetActiveTasks()
        {
            var jobs = await _jobService.GetActiveJobsAsync();
            return Ok(jobs);
        }
        /// <summary>
        /// Gets Jobs that are due.
        /// </summary>
        /// <param name="dueDate"></param>
        /// <param name="date">The job ID.</param>
        /// <returns>No content.</returns>
        [HttpGet("due/{dueDate}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetTasksByDueDate(DateTime dueDate)
        {
            var jobs = await _jobService.GetJobsByDueDateAsync(dueDate);
            if(jobs == null){
                return BadRequest();
            }
            return Ok(jobs);
        }
    }
}