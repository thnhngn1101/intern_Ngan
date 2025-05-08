using BPMaster.Domains.Entities;
using Common.Databases;
using Common.Repositories;
using Dapper;
using System.Data;

namespace DotNetTraining.Repositories
{
    public class MKTRepository(IDbConnection connection) : SimpleCrudRepository<MKT, Guid>(connection)
    {
        public async Task<(IEnumerable<MKT>, int totalCount)> GetAll(int pageNumber, int pageSize)
        {
            var offset = (pageNumber - 1) * pageSize;
            var sql = @"
                SELECT * FROM dbo.features
                ORDER BY ""Id"" DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*) FROM dbo.features;";

            using var multi = await connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize });
            var result = multi.Read<MKT>();
            var totalCount = multi.ReadSingle<int>();
            return (result, totalCount);
        }

       
    }
}
