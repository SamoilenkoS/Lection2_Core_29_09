using AutoMapper;
using Lection2_Core_29_09_API;
using Lection2_Core_BL.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_BL
{
    public class GoodsService
    {
        private readonly EfDbContext _dbContext;
        private readonly IMapper _mapper;

        public GoodsService(
            EfDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<GoodDto> CreateAsync(CreateGoodDto createGoodDto)
        {
            var good = _mapper.Map<Good>(createGoodDto);
            ValidateGood(good);

            good.Id = Guid.NewGuid();
            _dbContext.Goods.Add(good);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<GoodDto>(good);
        }


        public async Task<Good?> GetByIdAsync(Guid id)
            => await _dbContext.Goods.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<GoodDto>> GetAllAsync()
            => _mapper.Map<IEnumerable<GoodDto>>(await _dbContext.Goods.ToListAsync());

        public async Task<Good> DeleteAsync(Guid id)
        {
            var good = new Good { Id = id };
            _dbContext.Entry(good).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();

            return good;
        }

        public async Task<Good> UpdateAsync(Good good)
        {
            ValidateGood(good);

            _dbContext.Entry(good).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return good;
        }

        private static void ValidateGood(Good good)
        {
            if (good.Category == Category.Food &&
                good.Price > 200)
            {
                throw new ArgumentException(
                    $"{good.Price} is not valid for category {nameof(Category.Food)}");
            }
        }

    }
}