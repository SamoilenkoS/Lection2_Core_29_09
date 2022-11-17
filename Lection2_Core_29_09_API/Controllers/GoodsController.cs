using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services;
using Lection2_Core_DAL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class GoodsController : ControllerBase
{
    private ILogger<GoodsController> _logger;
    private readonly GoodsService _goodsService;

    public GoodsController(
        GoodsService goodsService,
        ILogger<GoodsController> logger)
    {
        _goodsService = goodsService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<GoodDto> CreateAsync(CreateGoodDto good)
    {
        _logger.LogInformation("Creating good with params: {@good}", good);
        return await _goodsService.CreateAsync(good);
    }

    [HttpGet]
    public async Task<IEnumerable<GoodDto>> GetAllAsync()
    {
        _logger.LogWarning("Oops! Someone is trying to get all goods!");
        return await _goodsService.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<GoodDto> GetByIdAsync(Guid id)
    {
        return await _goodsService.GetByIdAsync(id);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<GoodDto> DeleteAsync(Guid id)
        => await _goodsService.DeleteAsync(id);

    [HttpPut("{id}")]
    public async Task<GoodDto> UpdateAsync(Guid id, CreateGoodDto createGoodDto)
        => await _goodsService.UpdateAsync(id, createGoodDto);
}