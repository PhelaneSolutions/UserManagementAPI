using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserTasksAPI.DTO;
using UserTasksAPI.Models;

namespace UserTasksAPI.Interfaces
{
    public interface IUserService
    {
        
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> CreateUserAsync(User user);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<User> UpdateUserAsync(int id,User user);
        Task<bool> DeleteUserAsync(int id);
    }
}