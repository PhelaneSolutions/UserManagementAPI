using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserTasksAPI.Models;

namespace UserTasksAPI.Interfaces.Auth
{
    public interface ITokenService
    {
         string GenerateToken(User user);

    }
}