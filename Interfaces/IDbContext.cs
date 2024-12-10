using ClassLibrary.Enum;
using DatabaseDetail = ClassLibrary.BusinessObject.DatabaseConfig.DatabaseDetail;

namespace ClassLibrary.Interfaces;

public interface IDbContext
{
    string ConnectionName { get; set; }
    DatabaseDetail GetDatabaseConfig();
}