using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services.HashService;
using Lection2_Core_BL.Services.SmtpService;
using Lection2_Core_BL.Services.TokenService;
using Lection2_Core_DAL.Entities;
using Lection2_Core_DAL.Interfaces;
using Lection2_Core_DAL.RolesHelper;
using System.Text;

namespace Lection2_Core_BL.Services;

public class AuthService
{
    private readonly ISmtpService _smtpService;
    private readonly IHashService _hashService;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<EmailStatus> _emailStatusRepository;
    private readonly IGenericRepository<Role> _rolesRepository;
    private readonly IBasicGenericRepository<UserRoles> _userRolesRepository;
    private readonly ITokenService _tokenService;
    private readonly IRolesHelper _rolesHelper;
    private readonly IMapper _mapper;

    public AuthService(
        ISmtpService smtpService,
        IHashService hashService,
        IGenericRepository<EmailStatus> emailStatusRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> rolesRepository,
        IBasicGenericRepository<UserRoles> userRolesRepository,
        ITokenService tokenService,
        IRolesHelper rolesHelper,
        IMapper mapper)
    {
        _smtpService = smtpService;
        _hashService = hashService;
        _emailStatusRepository = emailStatusRepository;
        _userRepository = userRepository;
        _rolesRepository = rolesRepository;
        _userRolesRepository = userRolesRepository;
        _tokenService = tokenService;
        _rolesHelper = rolesHelper;
        _mapper = mapper;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string key)
    {
        var userWithRequiedKey = _emailStatusRepository.GetByPredicate(
            x => x.User!.Email == email && x.Key == key)
            .FirstOrDefault();
        if (userWithRequiedKey != null)
        {
            userWithRequiedKey.IsConfirmed = true;
            await _emailStatusRepository.UpdateAsync(userWithRequiedKey);
            var roleId = _rolesRepository.GetByPredicate(x
                => x.Title == RolesList.User)
                .Select(x => x.Id)
                .FirstOrDefault();
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
        var part1 = _userRepository.GetByPredicate(
            x => x.Email == credentialsDto.Login);
        var userWithRolesDto = part1.Select(x => new UserWithRolesDto
            {
                User = x,
                UserRoles = x.Roles.Select(x => x.Role.Title)
            })
            .FirstOrDefault();
        if (userWithRolesDto != null)
        {
            if (_hashService.VerifySameHash(credentialsDto.Password, userWithRolesDto.User.Password))
            {
                return _tokenService.GenerateToken(userWithRolesDto.User.Email, userWithRolesDto.UserRoles);
            }
        }

        return null;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, string role)
    {
        var roleId = _rolesHelper[role];
        return await _userRolesRepository.DeleteAsync(new UserRoles
        {
            RoleId = roleId,
            UserId = userId
        });
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