using AutoMapper;
using UserMicroService.dto;
using UserMicroService.Entities;
using UserMicroService.Profiles;

namespace UserMicroService.Tests;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_CreateUserDto_To_ApplicationUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto();

        // Act
        var applicationUser = _mapper.Map<ApplicationUser>(createUserDto);

        // Assert
        Assert.NotNull(applicationUser);

    }

    [Fact]
    public void Should_Map_ApplicationUser_To_ReadUserDto()
    {
        // Arrange
        var applicationUser = new ApplicationUser();

        // Act
        var readUserDto = _mapper.Map<ReadUserDto>(applicationUser);

        // Assert
        Assert.NotNull(readUserDto);
    
    }

    [Fact]
    public void Should_Map_UpdateUserDto_To_ApplicationUser()
    {
        // Arrange
        var updateUserDto = new UpdateUserDto();

        // Act
        var applicationUser = _mapper.Map<ApplicationUser>(updateUserDto);

        // Assert
        Assert.NotNull(applicationUser);
    }
}