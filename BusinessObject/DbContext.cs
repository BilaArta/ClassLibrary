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
        return _configuration.GetConnectionString(ConnectionName);
    }
}