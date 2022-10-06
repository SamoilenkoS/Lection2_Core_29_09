using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_DAL;
using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_BL.Services;

public class AuthService
{
    private readonly HashService _hashService;
    private readonly GenericRepository<User> _userRepository;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        HashService hashService,
        GenericRepository<User> userRepository,
        TokenService tokenService,
        IMapper mapper)
    {
        _hashService = hashService;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<string> RegisterAsync(RegistrationDto registrationDto)
    {
        var dto = _mapper.Map<User>(registrationDto);
        dto.Password = _hashService.GetHash(dto.Password);
        await _userRepository.CreateAsync(dto);

        return _tokenService.GenerateToken(registrationDto.Email, new List<string> { "Admin", "User" });
    }

    public async Task<string> LoginAsync(CredentialsDto credentialsDto)
    {
        var user = await _userRepository.GetByPredicateAsync(
            x => x.Email == credentialsDto.Login).FirstOrDefaultAsync();
        if(user != null)
        {
            if(_hashService.VerifySameHash(credentialsDto.Password, user.Password))
            {
                return _tokenService.GenerateToken(user.Email, new List<string> { "Admin", "User" });
            }
        }

        return null;
    }
}
