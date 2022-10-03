using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL;
using Lection2_Core_DAL.DTOs;

namespace Lection2_Core_BL.Services;

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

    public async Task<GoodDto> GetByIdAsync(Guid id)
        => _mapper.Map<GoodDto>(await _goodRepository.GetByIdAsync(id));

    public async Task<IEnumerable<GoodDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<GoodDto>>(await _goodRepository.GetAllAsync());

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
        if (good.Category == Category.Food &&
            good.Price > 200)
        {
            throw new ArgumentException(
                $"{good.Price} is not valid for category {nameof(Category.Food)}");
        }
    }
}