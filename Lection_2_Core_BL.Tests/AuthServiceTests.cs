using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Lection2_Core_BL.Services;
using Lection2_Core_BL.Services.HashService;
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

        //private User GetUser()
        //{
        //    return new User
        //    {
        //        Id = Guid.NewGuid(),
        //        Email = _fixture.Create<string>(),
        //        Password = _fixture.Create<string>(),
        //        EmailStatusId = _fixture.Create<Guid>(),
        //        EmailStatus = new EmailStatus
        //        {
        //            Id = _fixture.Create<Guid>(),
        //            Name = _fixture.Create<string>()
        //        }
        //    };
        //}
        //}

        private AuthService GetAuthService()
        {
            return new AuthService(_smtpService.Object, _hashService.Object,
                _emailStatusRepository.Object, _userRepository.Object,
                _rolesRepository.Object, _userRolesRepository.Object, _tokenService.Object,
                _rolesHelper.Object, _mapper.Object);
        }
    }
}
