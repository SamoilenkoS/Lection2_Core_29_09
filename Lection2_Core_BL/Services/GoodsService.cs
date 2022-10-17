using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL;
using Lection2_Core_DAL.DTOs;
using Lection2_Core_DAL.Interfaces;

namespace Lection2_Core_BL.Services;

public class GoodsService
{
    private readonly IGenericRepository<Good> _goodRepository;
    private readonly IMapper _mapper;

    public GoodsService(
        IGenericRepository<Good> goodRepository,
        IMapper mapper)
    {
        _goodRepository = goodRepository;
        _mapper = mapper;
    }

    public async Task<GoodDto> CreateAsync(CreateGoodDto createGoodDto)
    {
        var good = _mapper.Map<Good>(createGoodDto);
        ValidateGood(good);

        var response = await _goodRepository.CreateAsync(good);

        return _mapper.Map<GoodDto>(response);
    }

    public async Task<GoodDto> GetByIdAsync(Guid id)
    {
        var goodFromDb = await _goodRepository.GetByIdAsync(id);

        return _mapper.Map<GoodDto>(goodFromDb);
    }

    public async Task<IEnumerable<GoodDto>> GetAllAsync()
    {
        var goodFromDb = await _goodRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<GoodDto>>(goodFromDb);
    }

    public async Task<GoodDto> DeleteAsync(Guid id)
        => _mapper.Map<GoodDto>(await _goodRepository.DeleteAsync(id));

    public async Task<GoodDto> UpdateAsync(Guid id, CreateGoodDto createGoodDto)
    {
        var good = _mapper.Map<Good>(createGoodDto);
        good.Id = id;

        ValidateGood(good);

        return _mapper.Map<GoodDto>(await _goodRepository.UpdateAsync(good));
    }

    private static void ValidateGood(Good good)
    {
        good.Price = 55;
        if (good.Price < 0 || (good.Category == Category.Food &&
            good.Price > 200))
        {
            throw new ArgumentException(
                $"{good.Price} is not valid for category {nameof(Category.Food)}");
        }
    }
}