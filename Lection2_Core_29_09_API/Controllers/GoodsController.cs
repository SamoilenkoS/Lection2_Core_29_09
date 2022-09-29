using Lection2_Core_BL;
using Lection2_Core_BL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Lection2_Core_29_09_API.Controllers
{
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

        //[HttpGet]
        //public IEnumerable<Good> Get()
        //{
            
        //}
    }
}