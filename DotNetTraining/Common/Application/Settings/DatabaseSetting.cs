using Common.Application.Enums;

namespace Common.Application.Settings
{
    public class DatabaseSetting
    {
        public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class DatabaseWarehouseSetting
    {
        public DatabaseType DatabaseType { get; set; } = DatabaseType.Postgres;
        public string ConnectionString { get; set; } = string.Empty;
    }

}
