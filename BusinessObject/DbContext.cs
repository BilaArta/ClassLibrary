using ClassLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using ClassLibrary.Enum;
using DatabaseDetail = ClassLibrary.BusinessObject.DatabaseConfig.DatabaseDetail;
using ClassLibrary.Helper;
using Cassandra;

namespace ClassLibrary.BusinessObject;
public class DbContext: IDbContext
{
    private readonly IConfiguration _configuration;
    public string ConnectionName { get; set; }
    public DbContext(string context, IConfiguration configuration){
        ConnectionName = context;
        _configuration = configuration;
    }
    
    public DatabaseDetail GetDatabaseConfig(){
        var databaseConfig = _configuration.GetSection("DatabaseSettings").Get<Dictionary<string, DatabaseDetail>>();

        if (databaseConfig.TryGetValue(ConnectionName, out var selectedDb))
        {
            if (string.IsNullOrEmpty(selectedDb.ConnectionStrings))
            {
                LoggerHelper.Error($"Connection string for {ConnectionName} is missing!");
            }
            if (!selectedDb.DatabaseProvider.HasValue)
            {
                LoggerHelper.Error($"Database Provider for {ConnectionName} is missing!");
            }
            throw new Exception($"Database Provider/Connection string for {ConnectionName} is missing!");
        }else{
            LoggerHelper.Error($"Connection string for {ConnectionName} is missing!");
            throw new Exception($"Connection string for {ConnectionName} is missing!");
        }

        return selectedDb;
    }
}