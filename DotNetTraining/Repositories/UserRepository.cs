using System.Data;
using BPMaster.Domains.Entities;
using Common.Databases;
using Common.Repositories;
using Dapper;
using Dapper.Contrib.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using DotNetTraining.Domains.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNetTraining.Repositories
{
    public class UserRepository(IDbConnection connection) : SimpleCrudRepository<User, Guid>(connection)
    {
        public async Task<List<User>> GetAllUsers()
        {
            var sql = SqlCommandHelper.GetSelectSql<User>();
            var result = await connection.QueryAsync<User>(sql);
            return result.ToList();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            var param = new { Id = id };
            var sql = SqlCommandHelper.GetSelectSqlWithCondition<User>(new { Id = id });
            return await GetOneByConditionAsync(sql, param);

        }

        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                var sql = "SELECT * FROM users WHERE Email = @Email";
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetUserByEmail: {e.Message}");
                return null;
            }
        }

        public async Task<User?> Create(User user)
        {
            return await CreateAsync(user);
        }

        public async Task<User?> UpdateUser(User user)
        {
            return await UpdateAsync(user);
        }

        public async Task DeleteUser(User user)
        {
            await DeleteAsync(user);
        }
    }
}
