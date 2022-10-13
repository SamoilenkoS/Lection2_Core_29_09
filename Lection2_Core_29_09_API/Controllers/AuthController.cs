using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private const string ConfirmationRoute = "confirmation";
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegistrationDto registrationDto)
    {
        var controller = Request.RouteValues["controller"]!.ToString();
        var uriBuilder = new UriBuilder(
            Request.Scheme,
            Request.Host.Host,
            Request.Host.Port!.Value,
            controller + "/" + ConfirmationRoute);

        await _authService.RegisterAsync(registrationDto, uriBuilder);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(CredentialsDto credentialsDto)
    {
        return Ok(await _authService.LoginAsync(credentialsDto));
    }

    [HttpGet(ConfirmationRoute)]
    public async Task<IActionResult> ConfirmEmailAsync(string email, string key)
    {
        return Ok(await _authService.ConfirmEmailAsync(email, key));
    }
}
