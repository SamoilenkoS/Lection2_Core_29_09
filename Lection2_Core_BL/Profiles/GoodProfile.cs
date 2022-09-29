using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL;
using Lection2_Core_DAL.DTOs;

namespace Lection2_Core_BL.Profiles;

public class GoodProfile : Profile
{
    public GoodProfile()
    {
        CreateMap<CreateGoodDto, Good>()
            .ForMember(x => x.Category, opt => opt.MapFrom(
                c => Enum.Parse<Category>(c.Category)));
        
        CreateMap<Good, GoodDto>()
            .ForMember(x => x.Category, opt => opt.MapFrom(
                c => c.Category.ToString()));
    }
}
