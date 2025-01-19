using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserTasksAPI.Models;

namespace UserTasksAPI.Interfaces
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(Job job);
        Task<Job> GetJobByIdAsync(int id);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> UpdateJobAsync(int id,Job job);
        Task<bool> DeleteJobAsync(int id);
        Task<IEnumerable<Job>> GetExpiredJobsAsync();
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetJobsByDueDateAsync(DateTime dueDate);

    }
}