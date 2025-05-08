using Application.Settings;
using System.Text;
using AutoMapper;
using Common.Application.CustomAttributes;
using Common.Services;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Domains.Models;
using DotNetTraining.Domains.Entities;
using DotNetTraining.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace DotNetTraining.Services
{
    [ScopedService]
    public class UserService(IServiceProvider services, ApplicationSetting setting, IDbConnection connection) : BaseService(services)
    {
        private readonly UserRepository _repo = new(connection);

        public async Task<List<UserModel>> GetAllUsers()
        {
            var users = await _repo.GetAllUsers();
            return _mapper.Map<List<UserModel>>(users);
        }

        public async Task<UserModel?> GetUserById(Guid userId)
        {
            var user = await _repo.GetUserById(userId);
            if (user == null) throw new Exception("User not found");

            return _mapper.Map<UserModel>(user);
        }

        public async Task<User?> UpdateUser(Guid userId, UserDto userDto)
        {
            var existingUser = await _repo.GetUserById(userId);
            if (existingUser == null)
                throw new Exception("User not found");

            var updated = _mapper.Map(userDto, existingUser);
            updated.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdateUser(updated);
        }

        public async Task DeleteUser(Guid userId)
        {
            var existingUser = await _repo.GetUserById(userId);
            if (existingUser == null)
                throw new Exception("User not found");

            await _repo.DeleteUser(existingUser);
        }

        public async Task<User?> CreateUser(UserDto newUser)
        {
            var existingUser = await _repo.GetUserByEmail(newUser.Email);
            if (existingUser != null)
                throw new Exception("Email đã tồn tại");

            var user = _mapper.Map<User>(newUser);

            // Gán các trường hệ thống
           
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.Role = string.IsNullOrWhiteSpace(newUser.Role) ? "user" : newUser.Role;

            // Hash mật khẩu
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, newUser.Password);

            return await _repo.Create(user);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _repo.GetUserByEmail(email);
        }
    }
}
