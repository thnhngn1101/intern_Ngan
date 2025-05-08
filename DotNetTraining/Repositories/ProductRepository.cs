using Common.Databases;
using Common.Repositories;
using Dapper;
using DotNetTraining.Domains.Entities;
using System.Data;

namespace DotNetTraining.Repositories
{
    public class ProductRepository(IDbConnection connection) : SimpleCrudRepository<Product, Guid>(connection)
    {
        public async Task<List<Product>> GetAllProduct()
        {
            var sql = SqlCommandHelper.GetSelectSql<Product>();
            var result = await connection.QueryAsync<Product>(sql);
            return result.ToList();
        }

        public async Task<Product?> GetProductById(Guid productId)
        {
            var param = new { Id = productId };
            var sql = SqlCommandHelper.GetSelectSqlWithCondition<Product>(new { Id = productId });
            return await GetOneByConditionAsync(sql, param);
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            return await UpdateAsync(product);
        }

        public async Task<Product?> CreateProduct(Product product)
        {
            return await CreateAsync(product);  

        }
        public async Task DeleteProduct(Product product)
        {
            await DeleteAsync(product);
        }
    }
}
