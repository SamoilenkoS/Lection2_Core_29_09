using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Lection2_Core_BL.DTOs;
using Lection2_Core_BL.Services;
using Lection2_Core_BL.Services.GeneratorService;
using Lection2_Core_BL.Services.HashService;
using Lection2_Core_BL.Services.QueryService;
using Lection2_Core_BL.Services.SmtpService;
using Lection2_Core_BL.Services.TokenService;
using Lection2_Core_DAL.Entities;
using Lection2_Core_DAL.Interfaces;
using Lection2_Core_DAL.RolesHelper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lection2_Core_BL.Tests
{
    internal sealed class AuthServiceTests
    {
        Mock<ISmtpService> _smtpService;
        Mock<IHashService> _hashService;
        Mock<IGenericRepository<EmailStatus>> _emailStatusRepository;
        Mock<IGenericRepository<User>> _userRepository;
        Mock<IGenericRepository<Role>> _rolesRepository;
        Mock<IBasicGenericRepository<UserRoles>> _userRolesRepository;
        Mock<ITokenService> _tokenService;
        Mock<IRolesHelper> _rolesHelper;
        Mock<IQueryService> _queryService;
        Mock<IGeneratorService> _generatorService;
        Mock<IMapper> _mapper;
        Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _smtpService = new Mock<ISmtpService>();
            _hashService = new Mock<IHashService>();
            _emailStatusRepository = new Mock<IGenericRepository<EmailStatus>>();
            _userRepository = new Mock<IGenericRepository<User>>();
            _rolesRepository = new Mock<IGenericRepository<Role>>();
            _userRolesRepository = new Mock<IBasicGenericRepository<UserRoles>>();
            _tokenService = new Mock<ITokenService>();
            _rolesHelper = new Mock<IRolesHelper>();
            _queryService = new Mock<IQueryService>();
            _generatorService = new Mock<IGeneratorService>();
            _mapper = new Mock<IMapper>();
            _fixture = new Fixture();
        }

        [Test]
        public async Task RemoveRoleAsync_WhenRoleExist_ShouldRemoveUserRole()
        {
            // Arrange
            var authService = GetAuthService();
            var userId = _fixture.Create<Guid>();
            var userRole = _fixture.Create<string>();
            var roleId = _fixture.Create<Guid>();
            _rolesHelper
                .Setup(x => x[userRole])
                .Returns(roleId)
                .Verifiable();
            _userRolesRepository.Setup(
                x => x.DeleteAsync(It.Is<UserRoles>(
                x => x.UserId == userId && x.RoleId == roleId)))
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            var result = await authService.RemoveRoleAsync(userId, userRole);

            // Assert
            result.Should().BeTrue();
            _rolesHelper.Verify();
            _userRolesRepository.Verify();
        }

        [Test]
        public async Task LoginAsync_WhenValidCredentials_ShouldReturnAccessToken()
        {
            // Arrange
            var authService = GetAuthService();
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var token = _fixture.Create<string>();
            var user = _fixture.Build<User>().Without(x => x.Roles).Create();
            var userRoles = _fixture.Build<UserRoles>()
                .Without(x => x.Role)
                .Without(x => x.User)
                .CreateMany(2);
            foreach (var userRole in userRoles)
            {
                userRole.Role = new Role
                {
                    Title = _fixture.Create<string>()
                };
            }
            user.Roles = userRoles;
            _userRepository.Setup(x => x.GetByPredicate(
                It.IsAny<Expression<Func<User,bool>>>()))
                .Returns(new List<User> { user }.AsQueryable())
                .Verifiable();
            _hashService.Setup(x => x.VerifySameHash(password, user.Password))
                .Returns(true)
                .Verifiable();
            var roleTitles = userRoles.Select(x => x.Role.Title).ToList();
            _tokenService.Setup(x => x.GenerateToken(
                user.Email,
                It.Is<IEnumerable<string>>(x => x.SequenceEqual(roleTitles)))
                )
                .Returns(token)
                .Verifiable();

            // Act
            var result = await authService.LoginAsync(new DTOs.CredentialsDto
            {
                Login = email,
                Password = password
            });

            // Assert
            result.Should().Be(token);
            _userRepository.Verify();
            _hashService.Verify();
            _tokenService.Verify();
        }

        [Test]
        public async Task LoginAsync_WhenInvalidCredentials_ShouldReturnNull()
        {
            // Arrange
            var authService = GetAuthService();
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            _userRepository.Setup(x => x.GetByPredicate(
                It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User> { }.AsQueryable())
                .Verifiable();

            // Act
            var result = await authService.LoginAsync(new DTOs.CredentialsDto
            {
                Login = email,
                Password = password
            });

            // Assert
            result.Should().BeNull();
            _userRepository.Verify();
            _hashService.Verify(x =>
                x.VerifySameHash(
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                    Times.Never);
            _tokenService.Verify(x => x.GenerateToken(
                It.IsAny<string>(),
                It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        [Test]
        public async Task RegisterAsync_WhenValidInput_ShouldRegisterUser()
        {
            var authService = GetAuthService();
            var registrationDto = _fixture.Create<RegistrationDto>();
            var userFromMapper = _fixture.Build<User>().Without(x => x.Roles).Create();
            var hashedPassword = _fixture.Create<string>();
            var emailKey = _fixture.Create<string>();
            var queryDictionary = _fixture.Create<Dictionary<string, string>>();
            var uriBuilder = new UriBuilder();
            _mapper.Setup(x => x.Map<User>(It.Is<RegistrationDto>(x => x.Email == registrationDto.Email)))
                .Returns(userFromMapper)
                .Verifiable();
            _hashService.Setup(x => x.GetHash(userFromMapper.Password))
                .Returns(hashedPassword)
                .Verifiable();
            _userRepository.Setup(x => x.CreateAsync(It.Is<User>(x => x.Id == userFromMapper.Id)))
                .Verifiable();
            _generatorService
                .Setup(x => x.GetRandomString())
                .Returns(emailKey)
                .Verifiable();
            _queryService.Setup(x => x.CreateQueryParams(userFromMapper.Email, emailKey))
                .Returns(queryDictionary)
                .Verifiable();
            _queryService.Setup(x => x.AddQueryParamsToUri(uriBuilder, queryDictionary))
                .Verifiable();
            _emailStatusRepository.Setup(x => x.CreateAsync(It.Is<EmailStatus>(x => x.UserId == userFromMapper.Id && !x.IsConfirmed && x.Key == emailKey)))
                .Verifiable();
            _smtpService.Setup(x => x.SendEmailAsync(userFromMapper.Email, It.IsAny<string>(),
                uriBuilder.Uri.ToString())).Verifiable();

            await authService.RegisterAsync(registrationDto, uriBuilder);

            _mapper.Verify();
            _hashService.Verify();
            _userRepository.Verify();
            _generatorService.Verify();
            _queryService.Verify();
            _emailStatusRepository.Verify();
            _smtpService.Verify();
        }

        private AuthService GetAuthService()
        {
            return new AuthService(_smtpService.Object, _hashService.Object,
                _emailStatusRepository.Object, _userRepository.Object,
                _rolesRepository.Object, _userRolesRepository.Object, _tokenService.Object,
                _rolesHelper.Object, _queryService.Object, _generatorService.Object, _mapper.Object);
        }
    }
}
