using Lection2_Core_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class ActionsController : ControllerBase
{
    [HttpGet(nameof(Unauthorized))]
    public IActionResult Unauthorized()
    {
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
