using OverblikPlus.Common;
using OverblikPlus.Models.Dtos.User;

namespace OverblikPlus.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<ReadUserDto>> GetAllUsers();
    Task<ReadUserDto> GetUserById(string id);
    Task<Result> CreateUser(CreateUserDto newUser);
    Task UpdateUser(string id, UpdateUserDto updateUserDto);
    Task DeleteUser(string id);
}