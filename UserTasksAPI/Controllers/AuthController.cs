using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserTasksAPI.Interfaces;
using UserTasksAPI.Interfaces.Auth;
using UserTasksAPI.Models;

namespace UserTasksAPI.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }
        /// <summary>
        /// Creates a new token for user.
        /// </summary>
        /// <returns>The created job.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
        {
            var user = await _userService.AuthenticateAsync(loginModel.Username, loginModel.Password);
            if (user == null)
                return Unauthorized();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }

    }
}