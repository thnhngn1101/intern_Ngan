using Common.Databases.Interfaces;

namespace Common.Databases.PostgresSql
{
    public class SqlServerDictionaryDataType : IDictionaryDataType
    {
        private static readonly Dictionary<Type, string> SqlServerMap = new()
        {
            { typeof(Guid), "UNIQUEIDENTIFIER   default newid()" },
            { typeof(Guid?), "UNIQUEIDENTIFIER   default newid()" },
            { typeof(string), "varchar(255)" },
            { typeof(DateTime), "datetime" },
            { typeof(byte[]), "bytea[]" },
            { typeof(bool), "boolean" },
            { typeof(int), "int" },
             
        };

        Dictionary<Type, string> IDictionaryDataType.Dictionary => SqlServerMap;
    }
}
