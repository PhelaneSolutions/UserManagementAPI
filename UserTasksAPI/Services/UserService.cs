using System.Collections.Generic;
using System.Threading.Tasks;
using UserTasksAPI.Data;
using UserTasksAPI.DTO;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Repositories;

namespace UserTasksAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _context; 


        public UserService(IUserRepository userRepository, ILogger<UserService> logger, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _logger = logger;
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user)); 
            }

            if (await _userRepository.UserExistsAsync(user.Username))
            {
                _logger.LogError($"User with username '{user.Username}' already exists.");
                return null; 
            }

            await _userRepository.CreateUserAsync(user);
            return user;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var userDto = await _userRepository.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return null;
            }

            var user = new UserDto
            {
                Username = userDto.Username,
                Email = userDto.Email,
            };

            return user;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> UpdateUserAsync(int id,User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var existingUser = await _context.Users.FindAsync(id); 
            if (existingUser == null)
            {
                _logger.LogError($"User with ID {id} not found."); 
                return null;
            }

            existingUser.Email = user.Email; 
            existingUser.Username = user.Username;
            existingUser.Password = user.Password;

            return await _userRepository.UpdateUserAsync(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || user.Password != password)
            {
                return null;
            }

            return user;
        }
    }
}