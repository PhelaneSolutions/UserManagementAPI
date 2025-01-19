using Microsoft.EntityFrameworkCore;
using UserTasksAPI.Data;
using UserTasksAPI.Models;

namespace UserTasksAPI.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobRepository> _logger;

        public JobRepository(ApplicationDbContext context, ILogger<JobRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
             try
            {
                return await _context.Jobs.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all jobs.");
                throw;
            }
        }

        public async Task<Job> GetJobByIdAsync(int id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    _logger.LogWarning("Job with id {Id} not found.", id);
                    return null;
                }
                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting job with id {Id}.", id);
                return null;
            }
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            try
            {
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a job.");
                throw;
            }
        }

        public async Task<Job> UpdateJobAsync(Job job)
        {
            try
            {
                _context.Jobs.Attach(job);
                _context.Entry(job).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating job with id {JobId}.", job.Id);
                throw;
            }
        }

        public async Task<bool> DeleteJobAsync(int id)
        {
            try
            {
                var job = await GetJobByIdAsync(id);
                    _context.Jobs.Remove(job);
                    await _context.SaveChangesAsync();
                    return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting job with id {Id}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetExpiredJobsAsync()
        {
            return await _context.Jobs.Where(t => t.DueDate < DateTime.Now).ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _context.Jobs.Where(t => t.DueDate >= DateTime.Now).ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByDueDateAsync(DateTime dueDate)
        {
            return await _context.Jobs.Where(t => t.DueDate.Date == dueDate.Date).ToListAsync();
        }
    }
}