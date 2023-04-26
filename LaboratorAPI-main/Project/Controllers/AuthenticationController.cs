using DataLayer.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Dtos;
using Core.Services;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public static User user = new User();

        private MyAuthenticationService authenticationService { get; set; }
        public AuthenticationController(MyAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            authenticationService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Id = (int)request.Id;
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = request.Role;

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!authenticationService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }

            string token = authenticationService.CreateToken(user);
            return Ok(token);
        }
    }
}
