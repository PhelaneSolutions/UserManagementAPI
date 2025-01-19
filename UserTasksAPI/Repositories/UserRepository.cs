using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserTasksAPI.Data;
using UserTasksAPI.DTO;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;

namespace UserTasksAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with id {UserId} not found.", id);
                    return null;
                }
                return new UserDto
                {
                    Username = user.Username,
                    Email = user.Email,
                    // map other properties as needed
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user with id {UserId}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var userDtos = users.Select(user => new UserDto
                {
                    Username = user.Username,
                    Email = user.Email,
                    // map other properties as needed
                });
                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all users.");
                throw;
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a user.");
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with id {UserId}.", user.Id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var userDto = await GetUserByIdAsync(id);
                if (userDto != null)
                {
                    var user = await _context.Users.FindAsync(id);
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with id {UserId}.", id);
                throw;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user with username {Username}.", username);
                throw;
            }
        }
   
        public async Task<User> AuthenticateAsync(string username, string password)


        {
            try
            {
                var user = await GetUserByUsernameAsync(username);
                if (user == null || user.Password != password)
                {
                    _logger.LogWarning("Authentication failed for user with username {Username}.", username);
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while authenticating user with username {Username}.", username);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username); 
        }
    }
}