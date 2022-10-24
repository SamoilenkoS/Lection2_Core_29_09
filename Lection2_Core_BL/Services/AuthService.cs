using AutoMapper;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services.GeneratorService;
using Lection2_Core_BL.Services.HashService;
using Lection2_Core_BL.Services.QueryService;
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
    private readonly IQueryService _queryService;
    private readonly IGeneratorService _generatorService;
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
        IQueryService queryService,
        IGeneratorService generatorService,
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
        _queryService = queryService;
        _generatorService = generatorService;
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
        var emailKey = _generatorService.GetRandomString();
        _queryService.AddQueryParamsToUri(uriBuilder, _queryService.CreateQueryParams(dto.Email, emailKey));
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
}