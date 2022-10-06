using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegistrationDto registrationDto)
        {
            return Ok(await _authService.RegisterAsync(registrationDto));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(CredentialsDto credentialsDto)
        {
            return Ok(await _authService.LoginAsync(credentialsDto));
        }
    }
}
