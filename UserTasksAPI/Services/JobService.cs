using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserTasksAPI.Data;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Repositories;

namespace UserTasksAPI.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _job;
        private readonly ILogger<UserRepository> _logger;
        private readonly ApplicationDbContext _context; 

        public JobService(IJobRepository job, ILogger<UserRepository> logger, ApplicationDbContext context)
        {
            _job = job;
            _logger = logger;
            _context = context;
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }
        
            if (job.Assignee <= 0) 
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == job.Assignee); 
                if (!userExists) 
                {
                    _logger.LogError("Assignee ID {AssigneeId} does not correspond to an existing user.", job.Assignee); 
                    return null; 
                }
            }


            return await _job.CreateJobAsync(job);
        }

        public async Task<Job> GetJobByIdAsync(int id)
        {
            return await _job.GetJobByIdAsync(id);
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _job.GetAllJobsAsync();
        }

        public async Task<Job> UpdateJobAsync(int id,Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            var existingJob = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id); 
            if (existingJob == null)
            {
                _logger.LogError($"Job with ID {id} not found."); 
                return null;
            }

            existingJob.Title = job.Title; 
            existingJob.Description = job.Description;
            existingJob.Assignee = job.Assignee; 
            existingJob.Description = job.Description;
            existingJob.DueDate = job.DueDate;

            return await _job.UpdateJobAsync(existingJob);
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            var jobExists = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
            if(jobExists != null)
            {
             return await _job.DeleteJobAsync(id);
            }
            return false;
        }

        public async Task<IEnumerable<Job>> GetExpiredJobsAsync()
        {
            return await _job.GetExpiredJobsAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _job.GetActiveJobsAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByDueDateAsync(DateTime dueDate)
        {
            if (!DateTime.TryParse(dueDate.ToString(), out DateTime validDueDate))
            {
                _logger.LogError($"Invalid due date format: {dueDate}"); 
                return null;
            }
            return await _job.GetJobsByDueDateAsync(validDueDate);
        }
    }
}