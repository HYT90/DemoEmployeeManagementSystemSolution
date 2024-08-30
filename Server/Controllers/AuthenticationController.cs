using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUserAccount userAccount) : Controller
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null) return BadRequest("Model is empty");
            var res = await userAccount.CreateAsync(user);
            return Ok(res);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null) return BadRequest("Model is empty");
            var res = await userAccount.SignInAsync(user);
            return Ok(res);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
        {
            if (token == null) return BadRequest("Model is empty");
            var res = await userAccount.RefreshTokenAsync(token);
            return Ok(res);
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUserAsync()
        {
            var users = await userAccount.GetUsers();
            if (users is null) return NotFound();
            return Ok(users);
        }

        [HttpGet("roles")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await userAccount.GetRoles();
            if (roles is null) return NotFound();
            return Ok(roles);
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var res = await userAccount.DeleteUser(id);
            return Ok(res);
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(ManageUser user)
        {
            var res = await userAccount.UpdateUser(user);
            return Ok(res);
        }

        [HttpGet("user-image/{id}")]
        public async Task<IActionResult> GetUserImage(int id)
        {
            var result = await userAccount.GetUserImage(id);
            return Ok(result);
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfiler(UserProfile profile)
        {
            var res = await userAccount.UpdateProfile(profile);
            return Ok(res);
        }
    }
}
