using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services;
using Lection2_Core_DAL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_API.Controllers;

[ApiController]
[Route("[controller]")]
public class GoodsController : ControllerBase
{
    private readonly GoodsService _goodsService;

    public GoodsController(GoodsService goodsService)
    {
        _goodsService = goodsService;
    }

    [HttpPost]
    public async Task<GoodDto> CreateAsync(CreateGoodDto good)
    {
        return await _goodsService.CreateAsync(good);
    }

    [HttpGet]
    public async Task<IEnumerable<GoodDto>> GetAllAsync()
    {
        return await _goodsService.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<GoodDto> GetByIdAsync(Guid id)
    {
        return await _goodsService.GetByIdAsync(id);
    }

    [HttpDelete("{id}")]
    public async Task<GoodDto> DeleteAsync(Guid id)
        => await _goodsService.DeleteAsync(id);

    [HttpPut("{id}")]
    public async Task<GoodDto> UpdateAsync(Guid id, CreateGoodDto createGoodDto)
        => await _goodsService.UpdateAsync(id, createGoodDto);
}