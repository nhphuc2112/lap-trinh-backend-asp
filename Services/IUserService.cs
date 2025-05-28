using TodoList.API.Models;
using TodoList.API.DTOs;

namespace TodoList.API.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(int id);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
    }
} 