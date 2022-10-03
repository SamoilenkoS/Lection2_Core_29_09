using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL.Entities;

namespace Lection2_Core_BL.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegistrationDto, User>()
           .ForMember(x => x.CreatedAt, opt => opt.MapFrom(
               c => DateTime.Now.Date));
    }
}
