using Common.Application.Settings;
using Common.Databases.Interfaces;

namespace Common.Databases.PostgresSql
{
    public class SqlServerGenerator: SqlGenerator
    {
        protected override IDictionaryDataType DictionaryDataType => new SqlServerDictionaryDataType();
    }
}
