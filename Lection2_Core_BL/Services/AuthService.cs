using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services.SmtpService;
using Lection2_Core_DAL;
using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web;

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
        var userWithRequiedKey = await _emailStatusRepository.GetByPredicateAsync(
            x => x.User!.Email == email && x.Key == key)
            .FirstOrDefaultAsync();
        if(userWithRequiedKey != null)
        {
            userWithRequiedKey.IsConfirmed = true;
            await _emailStatusRepository.UpdateAsync(userWithRequiedKey);
            return true;
        }

        return false;
    }

    public async Task<string> RegisterAsync(
        RegistrationDto registrationDto, UriBuilder uriBuilder)
    {
        var dto = _mapper.Map<User>(registrationDto);
        dto.Password = _hashService.GetHash(dto.Password);
        await _userRepository.CreateAsync(dto);
        var emailKey = GetRandomString();
        AddQueryParamsToUri(uriBuilder, CreateQueryParams(dto.Email, emailKey));
        await _emailStatusRepository.CreateAsync(
            new EmailStatus
            {
                UserId = dto.Id,
                IsConfirmed = false,
                Key = emailKey
            });

        await _smtpService.SendEmailAsync(dto.Email, "Email confirmation", uriBuilder.Uri.ToString());

        return _tokenService.GenerateToken(registrationDto.Email, new List<string> { "Admin", "User" });
    }

    private static void AddQueryParamsToUri(UriBuilder uriBuilder, Dictionary<string, string> queryParams)
    {
        uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    private static Dictionary<string, string> CreateQueryParams(string email, string emailKey)
    {
        var queryParams = new Dictionary<string, string>();
        queryParams.Add("email", email);
        queryParams.Add("key", emailKey);
        return queryParams;
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
        var random = new Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower();
        var length = random.Next(10, 20);
        var result = new StringBuilder(string.Empty, length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
}
