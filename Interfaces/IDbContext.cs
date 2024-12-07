namespace ClassLibrary.Interfaces;

public interface IDbContext
{
    string ConnectionName { get; set; }
    string GetConnectionString();
}