using Application.Settings;
using System.Text;
using AutoMapper;
using Common.Application.CustomAttributes;
using Common.Services;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Domains.Models;
using DotNetTraining.Domains.Entities;
using DotNetTraining.Repositories;
using Newtonsoft.Json;
using System.Data;
using iText.Commons.Actions.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.Forms.Fields.Merging;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.AspNetCore.Identity;
namespace DotNetTraining.Services
{
  
    [ScopedService]
    public class UserService(IServiceProvider services, ApplicationSetting setting, IDbConnection connection): BaseService(services)
    {
        private readonly UserRepository _repo = new(connection);

        public async Task<List<UserModel>> GetAllUsers()
        {
            var users = await _repo.GetAllUsers();

            var result = _mapper.Map<List<UserModel>>(users);

            return result;

        }

        public async Task<UserModel?> GetUserById(Guid userId)
        {
            var existingUser = await _repo.GetUserById(userId);
            if (existingUser == null)
            {
                throw new Exception("user not exist");
            }
            // map entity to Dto
            var dto = _mapper.Map<UserModel>(existingUser);

            return dto;

        }

        public async Task<User?> UpdateUser(Guid userId, UserDto userDto)
        {
            var existingUser = await _repo.GetUserById(userId);
            if (existingUser == null)   
            {
                throw new Exception(" id not found"); // User không tồn tại
            }
            
            var user = _mapper.Map(userDto, existingUser); 

            var updatedUser = await _repo.UpdateUser(user);

            return user;
        }
        public async Task DeleteUser(Guid userId)
        {
            var existingUser = await _repo.GetUserById(userId);

            if (existingUser == null)
            {
                throw new Exception("user not exist"); // User không tồn tại
            }

            await _repo.DeleteUser(existingUser);
        }

        public async Task<User?> CreateUser(UserDto newUser)
        {

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _repo.GetUserByEmail(newUser.Email);
            if (existingUser != null)
            {
                throw new Exception("email đã tồn tại"); // Email đã tồn tại
            }
            // Tạo đối tượng User
            var user = _mapper.Map<User>(newUser);
            //user.Id = Guid.NewGuid();

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, newUser.Password);

            // Gọi repository để lưu vào DB
            return await _repo.Create(user);
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _repo.GetUserByEmail(email); // đã có hàm này ở repository
        }


    }
}
