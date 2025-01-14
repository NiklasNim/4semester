using UserMicroService.Common;
using UserMicroService.dto;

namespace UserMicroService.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<ReadUserDto>>> GetAllUsersAsync();
        Task<Result<ReadUserDto>> GetUserById(string id, string userRole);
        Task<Result<string>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Result> DeleteUserAsync(string id);
        Task<Result> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
    }
}