using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using ClassLibrary.Interfaces;

namespace ClassLibrary.DbConnection;
public class DbConnection{
    private readonly string Context;
    private readonly IConfiguration _iconfig;
    public DbConnection(IDbContext context){

    }
}