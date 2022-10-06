using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services.SmtpService;
using Lection2_Core_DAL;
using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lection2_Core_BL.Services;

public class AuthService
{
    private readonly ISmtpService _smtpService;
    private readonly HashService _hashService;
    private readonly GenericRepository<User> _userRepository;
    private readonly GenericRepository<EmailStatus> _emailStatusRepository;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        ISmtpService smtpService,
        HashService hashService,
        GenericRepository<EmailStatus> emailStatusRepository,
        GenericRepository<User> userRepository,
        TokenService tokenService,
        IMapper mapper)
    {
        _smtpService = smtpService;
        _hashService = hashService;
        _emailStatusRepository = emailStatusRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string key)
    {
        var user = await _userRepository.GetByPredicateAsync(x => x.Email == email)
            .FirstOrDefaultAsync();
        if(user != null)
        {
            var emailStatus = await _emailStatusRepository.GetByPredicateAsync(x => x.UserId == user.Id)
                .FirstOrDefaultAsync();
            if(emailStatus.Key == key)
            {
                emailStatus.IsConfirmed = true;
                await _emailStatusRepository.UpdateAsync(emailStatus);
                return true;
            }
        }

        return false;
    }

    public async Task<string> RegisterAsync(RegistrationDto registrationDto)
    {
        var dto = _mapper.Map<User>(registrationDto);
        dto.Password = _hashService.GetHash(dto.Password);
        await _userRepository.CreateAsync(dto);
        await _emailStatusRepository.CreateAsync(
            new EmailStatus
        {
            UserId= dto.Id,
            IsConfirmed = false,
            Key = GetRandomString()
        });

        await _smtpService.SendEmailAsync(dto.Email, "Email confirmation", GetRandomString());

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

    private static string GetRandomString()
    {
        return "asdwdqeqwe!!ewe4$";
    }
}
