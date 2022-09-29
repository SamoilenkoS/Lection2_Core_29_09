using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL;
using Lection2_Core_DAL.DTOs;

namespace Lection2_Core_BL;

public class GoodsService
{
    private readonly GenericRepository<Good> _goodRepository;
    private readonly IMapper _mapper;

    public GoodsService(
        GenericRepository<Good> goodRepository,
        IMapper mapper)
    {
        _goodRepository = goodRepository;
        _mapper = mapper;
    }

    public async Task<GoodDto> CreateAsync(CreateGoodDto createGoodDto)
    {
        var good = _mapper.Map<Good>(createGoodDto);
        ValidateGood(good);

        await _goodRepository.CreateAsync(good);
        
        return _mapper.Map<GoodDto>(good);
    }


    //public async Task<Good?> GetByIdAsync(Guid id)
    //    => await _dbContext.Goods.SingleOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<GoodDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<GoodDto>>(await _goodRepository.GetAllAsync());

    //public async Task<Good> DeleteAsync(Guid id)
    //{
    //    var good = new Good { Id = id };
    //    _dbContext.Entry(good).State = EntityState.Deleted;
    //    await _dbContext.SaveChangesAsync();

    //    return good;
    //}

    //public async Task<Good> UpdateAsync(Good good)
    //{
    //    ValidateGood(good);

    //    _dbContext.Entry(good).State = EntityState.Modified;
    //    await _dbContext.SaveChangesAsync();

    //    return good;
    //}

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