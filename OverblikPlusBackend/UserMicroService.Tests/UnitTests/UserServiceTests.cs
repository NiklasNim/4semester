using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using OverblikPlus.Shared.Interfaces;
using UserMicroService.Common;
using UserMicroService.Controllers;
using UserMicroService.DataAccess;
using UserMicroService.dto;
using UserMicroService.Entities;
using UserMicroService.Helpers;
using UserMicroService.Services;
using Xunit;

namespace UserMicroService.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = MockUserManager(new List<ApplicationUser>());
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _userService = new UserService(_userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnList_WhenUsersExist()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", FirstName = "John", LastName = "Doe" }
            }.AsQueryable();

            var userDtos = new List<ReadUserDto>
            {
                new ReadUserDto { Id = "1", FirstName = "John", LastName = "Doe" }
            };

            _userManagerMock.Setup(x => x.Users).Returns(users.BuildMock().BuildMockDbSet().Object);
            _mapperMock.Setup(x => x.Map<List<ReadUserDto>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal("John", result.Data.First().FirstName);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            EncryptionHelper.SetEncryptionKey("TestEncryptionKey123456123456789");

            var user = new ApplicationUser
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Medication = EncryptionHelper.Encrypt("Med1")
            };
            var userDto = new ReadUserDto
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<ReadUserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserById("1", "Admin");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("John", result.Data.FirstName);
            Assert.Equal("Doe", result.Data.LastName);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnUserId_WhenSuccessful()
        {
            // Arrange
            EncryptionHelper.SetEncryptionKey("TestEncryptionKey123456123456789");
            var createUserDto = new CreateUserDto
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123",
                Medication = "Med1"
            };

            var user = new ApplicationUser
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Medication = EncryptionHelper.Encrypt("Med1")
            };

            _mapperMock.Setup(x => x.Map<ApplicationUser>(createUserDto)).Returns(user);
            _userManagerMock.Setup(x => x.CreateAsync(user, createUserDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("1", result.Data);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1" };
            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.DeleteUserAsync("1");

            // Assert
            Assert.True(result.Success);
            _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            EncryptionHelper.SetEncryptionKey("TestEncryptionKey123456123456789");

            var user = new ApplicationUser
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Medication = EncryptionHelper.Encrypt("Med1", true),
                Goals = "Goal1"
            };

            var updateUserDto = new UpdateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Medication = "Med2",
                Goals = "Goal2"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map(It.IsAny<UpdateUserDto>(), It.IsAny<ApplicationUser>()))
                .Callback<UpdateUserDto, ApplicationUser>((dto, u) =>
                {
                    u.FirstName = dto.FirstName;
                    u.LastName = dto.LastName;
                    u.Email = dto.Email;
                    u.Medication = EncryptionHelper.Encrypt(dto.Medication, true);
                    u.Goals = dto.Goals;
                });

            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdateUserAsync("1", updateUserDto);

            // Assert
            Assert.True(result.Success);
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);

            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.Equal("jane.smith@example.com", user.Email);
            Assert.Equal("Med2", EncryptionHelper.Decrypt(user.Medication, true));
            Assert.Equal("Goal2", user.Goals);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager(List<ApplicationUser> users)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            userManager.Setup(x => x.Users).Returns(users.AsQueryable());
            return userManager;
        }

        [Fact]
        public void CanAssignBostedIdToUser()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new UserDbContext(options))
            {
                var user = new ApplicationUser
                {
                    UserName = "testUser",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "tes",
                    Medication = "Med1",
                    Role = "User",
                    BostedId = null
                };

                context.Users.Add(user);
                context.SaveChanges();

                user.BostedId = 1;
                context.SaveChanges();

                var updatedUser = context.Users.Find(user.Id);
                Assert.NotNull(updatedUser);
                Assert.Equal(1, updatedUser.BostedId);
            }
        }

        [Fact]
        public async Task GetAllUsersInBosted_Should_Return_OkResult_When_User_Has_Same_BostedId()
        {
            // Arrange
            int bostedId = 1;
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", FirstName = "John", LastName = "Doe", BostedId = bostedId }
            };
            _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable().BuildMock().BuildMockDbSet().Object);

            var createUserValidatorMock = new Mock<IValidator<CreateUserDto>>();
            var updateUserValidatorMock = new Mock<IValidator<UpdateUserDto>>();
            var loggerMock = new Mock<ILoggerService>();

            var userService = new UserService(_userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
            var controller = new UserController(userService, createUserValidatorMock.Object, updateUserValidatorMock.Object, loggerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("bostedId", bostedId.ToString())
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetAllUsersInBosted(bostedId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Result<List<ApplicationUser>>>(okResult.Value);
            Assert.IsType<List<ApplicationUser>>(resultValue.Data);
        }

        [Fact]
        public async Task GetAllUsersInBosted_Should_Return_Forbidden_When_User_Has_Different_BostedId()
        {
            // Arrange
            int bostedId = 2;
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", FirstName = "John", LastName = "Doe", BostedId = 1 }
            };
            _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable().BuildMock().BuildMockDbSet().Object);

            var createUserValidatorMock = new Mock<IValidator<CreateUserDto>>();
            var updateUserValidatorMock = new Mock<IValidator<UpdateUserDto>>();
            var loggerMock = new Mock<ILoggerService>();

            var userService = new UserService(_userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
            var controller = new UserController(userService, createUserValidatorMock.Object, updateUserValidatorMock.Object, loggerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("bostedId", "1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetAllUsersInBosted(bostedId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}