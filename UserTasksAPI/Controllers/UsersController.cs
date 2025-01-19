using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Models;
using UserTasksAPI.Repositories;
using UserTasksAPI.Services;

namespace UserTasksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepo;

        public UsersController(IUserService userService, IUserRepository userRepo)
        {
            _userService = userService;
            _userRepo = userRepo;
        }
        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>The created user.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateUser(User user)
        {  
            var createdUser = await _userService.CreateUserAsync(user);
            if(createdUser == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="user">The updated user.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if(existingUser == null)
            {
                return NotFound();
            }

            await _userService.UpdateUserAsync(id,user);
            return Ok("Deleted Successfully");
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }
    }
}