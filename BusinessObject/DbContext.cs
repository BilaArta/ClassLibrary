using ClassLibrary.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ClassLibrary.BusinessObject;
public class DbContext: IDbContext
{
    private readonly IConfiguration _configuration;

    public string ConnectionName { get; set; }
    public DbContext(string context, IConfiguration configuration){
        ConnectionName = context;
        _configuration = configuration;
    }
    
    public string GetConnectionString(){
        var IsUsingSecretManager = AppEnv.IsUsingSecretManager;
        return IsUsingSecretManager ? Environment.GetEnvironmentVariable(ConnectionName):  _configuration.GetConnectionString(ConnectionName);
    }
}