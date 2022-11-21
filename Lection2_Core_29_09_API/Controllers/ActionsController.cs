using Lection2_Core_BL.Services;
using Lection2_Core_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ActionsController : ControllerBase
{
    private readonly CachingService _cashingService;

    public ActionsController(CachingService cashingService)
    {
        _cashingService = cashingService;
    }

    [HttpGet(nameof(Unauthorized))]
    public async Task<IActionResult> Unauthorized()
    {
        var value = await _cashingService.GetAsync("3");
        await _cashingService.SaveAsync("10", "50");
        return Ok();
    }

    [Authorize]
    [HttpGet(nameof(Authorized))]
    public IActionResult Authorized()
    {
        return Ok();
    }

    [Authorize(Roles = RolesList.Admin)]
    [HttpGet(nameof(Admin))]
    public IActionResult Admin()
    {
        return Ok();
    }

    [Authorize(Roles = RolesList.User)]
    [HttpGet(nameof(User))]
    public IActionResult User()
    {
        return Ok();
    }

    [Authorize(Roles = RolesList.Admin)]
    [Authorize(Roles = RolesList.User)]
    [HttpGet(nameof(UserAndAdmin))]
    public IActionResult UserAndAdmin()
    {
        return Ok();
    }

    [Authorize(Roles = RolesList.User + "," + RolesList.Admin)]
    [HttpGet(nameof(UserOrAdmin))]
    public IActionResult UserOrAdmin()
    {
        return Ok();
    }
}
