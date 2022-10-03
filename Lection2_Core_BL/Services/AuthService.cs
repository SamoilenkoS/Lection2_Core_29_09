using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL;
using Lection2_Core_DAL.Entities;

namespace Lection2_Core_BL.Services;

public class AuthService
{
    private readonly GenericRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public AuthService(
        GenericRepository<User> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task RegisterAsync(RegistrationDto registrationDto)
    {
        //validate
        //hash password
        await _userRepository.CreateAsync(_mapper.Map<User>(registrationDto));
        //save to db
        //return token;
    }
}
