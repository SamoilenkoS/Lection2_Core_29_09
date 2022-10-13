using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services.SmtpService;
using Lection2_Core_DAL;
using Lection2_Core_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Lection2_Core_BL.Services;

public class AuthService
{
    private readonly ISmtpService _smtpService;
    private readonly HashService _hashService;
    private readonly GenericRepository<User> _userRepository;
    private readonly GenericRepository<EmailStatus> _emailStatusRepository;
    private readonly GenericRepository<Role> _rolesRepository;
    private readonly BasicGenericRepository<UserRoles> _userRolesRepository;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(
        ISmtpService smtpService,
        HashService hashService,
        GenericRepository<EmailStatus> emailStatusRepository,
        GenericRepository<User> userRepository,
        GenericRepository<Role> rolesRepository,
        BasicGenericRepository<UserRoles> userRolesRepository,
        TokenService tokenService,
        IMapper mapper)
    {
        _smtpService = smtpService;
        _hashService = hashService;
        _emailStatusRepository = emailStatusRepository;
        _userRepository = userRepository;
        _rolesRepository = rolesRepository;
        _userRolesRepository = userRolesRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string key)
    {
        var userWithRequiedKey = await _emailStatusRepository.GetByPredicateAsync(
            x => x.User!.Email == email && x.Key == key)
            .FirstOrDefaultAsync();
        if (userWithRequiedKey != null)
        {
            userWithRequiedKey.IsConfirmed = true;
            await _emailStatusRepository.UpdateAsync(userWithRequiedKey);
            var roleId = await _rolesRepository.GetByPredicateAsync(x
                => x.Title == RolesList.User)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            await _userRolesRepository.CreateAsync(new UserRoles
            {
                UserId = userWithRequiedKey.UserId,
                RoleId = roleId
            });

            return true;
        }

        return false;
    }

    public async Task RegisterAsync(
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

    }

    public async Task<string> LoginAsync(CredentialsDto credentialsDto)
    {
        var userWithRolesDto = await _userRepository.GetByPredicateAsync(
            x => x.Email == credentialsDto.Login)
            .Select(x => new UserWithRolesDto
            {
                User = x,
                UserRoles = x.Roles.Select(x => x.Role.Title)
            })
            .FirstOrDefaultAsync();
        if (userWithRolesDto != null)
        {
            if (_hashService.VerifySameHash(credentialsDto.Password, userWithRolesDto.User.Password))
            {
                return _tokenService.GenerateToken(userWithRolesDto.User.Email, userWithRolesDto.UserRoles);
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

}