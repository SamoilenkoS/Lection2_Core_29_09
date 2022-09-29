using AutoMapper;
using Lection2_Core_29_09_API;
using Lection2_Core_BL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Profiles
{
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
}
