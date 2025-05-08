using System.Data;
using System.Data.SqlClient;
using BPMaster.Common.Databases.Interfaces;
using Common.Application.Enums;
using Common.Application.Settings;
using Npgsql;


namespace Common.Databases
{
    public class DbConnectionFactory
    {

        public static IDbConnection GetConnection(DatabaseSetting setting)
        {
            IDbConnection connection = setting.DatabaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(setting.ConnectionString),
                DatabaseType.Postgres => new NpgsqlConnection(setting.ConnectionString),
                _ => throw new NotImplementedException(),
            };

            return connection;
        }
        public static IBPMasterDbConnection GetBPMasterConnection(DatabaseSetting setting)
        {
            IDbConnection connection = setting.DatabaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(setting.ConnectionString),
                DatabaseType.Postgres => new NpgsqlConnection(setting.ConnectionString),
                _ => throw new NotImplementedException(),
            };

            return new BPMasterDbConnectionWrapper(connection);
        }

        public static IWarehouseDbConnection GetWarehouseConnection(DatabaseWarehouseSetting setting)
        {
            IDbConnection connection = setting.DatabaseType switch
            {
                DatabaseType.SqlServer => new SqlConnection(setting.ConnectionString),
                DatabaseType.Postgres => new NpgsqlConnection(setting.ConnectionString),
                _ => throw new NotImplementedException(),
            };

            return new WarehouseDbConnectionWrapper(connection);
        }


    }
}
