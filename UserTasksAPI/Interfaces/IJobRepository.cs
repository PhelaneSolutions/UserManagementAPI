using UserTasksAPI.Models;

namespace UserTasksAPI.Repositories
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> GetJobByIdAsync(int id);
        Task<Job> CreateJobAsync(Job job);
        Task<Job> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(int id);
        Task<IEnumerable<Job>> GetExpiredJobsAsync();
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetJobsByDueDateAsync(DateTime dueDate);
    }
}