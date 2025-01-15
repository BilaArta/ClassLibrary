using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Cassandra;
using Dapper;
using Npgsql;
using MySql.Data.MySqlClient;
using ClassLibrary.Enum;
using ClassLibrary.BusinessObject;
using ClassLibrary.Helper;
using Microsoft.Extensions.Configuration;
using DatabaseDetail = ClassLibrary.BusinessObject.DatabaseConfig.DatabaseDetail;
using MySqlX.XDevAPI.Common;

namespace ClassLibrary.RepositoryBase;
public abstract class BaseRepository<T> : DbContext, IDisposable where T : class
{
    private readonly DatabaseDetail _dbDetailConfig;
    private IDbConnection _connection;
    private readonly string setIsolationLevelRead = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
    protected BaseRepository(string DbContext, IConfiguration configuration) : base(DbContext, configuration)
    {
        _dbDetailConfig = GetDatabaseConfig();
    }

    protected IDbConnection Connection
    {
        get
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = (EnumDatabaseProvider)_dbDetailConfig.DatabaseProvider switch
                {
                    EnumDatabaseProvider.SqlServer => new SqlConnection(_dbDetailConfig.ConnectionStrings),
                    EnumDatabaseProvider.MySql => new MySqlConnection(_dbDetailConfig.ConnectionStrings),
                    EnumDatabaseProvider.Npgsql => new NpgsqlConnection(_dbDetailConfig.ConnectionStrings),
                    _ => throw new NotSupportedException($"Database string Config {ConnectionName} is not supported.")
                };
            }
            //_connection.Open();
            if (_connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Koneksi berhasil dibuka!");
            }
            else
            {
                Console.WriteLine($"Status koneksi: {_connection.State}");
            }
            return _connection;
        }
    }

    protected (IDbConnection connection, IDbTransaction transaction) ConnectionTransaction
    {
        get
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = (EnumDatabaseProvider)_dbDetailConfig.DatabaseProvider switch
                {
                    EnumDatabaseProvider.SqlServer => new SqlConnection(_dbDetailConfig.ConnectionStrings),
                    EnumDatabaseProvider.MySql => new MySqlConnection(_dbDetailConfig.ConnectionStrings),
                    EnumDatabaseProvider.Npgsql => new NpgsqlConnection(_dbDetailConfig.ConnectionStrings),
                    _ => throw new NotSupportedException($"Database string Config {ConnectionName} is not supported.")
                };
            }
            //_connection.Open();
            if (_connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Koneksi berhasil dibuka!");
            }
            else
            {
                Console.WriteLine($"Status koneksi: {_connection.State}");
            }
            using var transaction = _connection.BeginTransaction();
            return (_connection, transaction);
        }
    }

    // INSERT Data
    public async Task<int> InsertAsyncTransaction(string query, object parameters)
    {
        try
        {
            var db = ConnectionTransaction;
            db.connection.Execute(setIsolationLevelRead, db.transaction);

            try
            {
                var result = await db.connection.ExecuteAsync(query, parameters, db.transaction);
                LoggerHelper.Info($"Data created :{result.ToString()}");
                db.transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                LoggerHelper.Error($"InsertAsyncTransaction::Exception Error:{e}");
                throw new Exception($"Error executing InsertAsync: {e.Message}", e);
            }

        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"InsertAsyncTransaction::Exception Error ex:{ex}");
            throw new Exception($"Error executing InsertAsync: {ex.Message}", ex);
        }
    }

    // INSERT Data
    public async Task<int> InsertAsync(string query, object parameters = null)
    {
        try
        {
            return await Connection.ExecuteAsync(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing InsertAsync: {ex.Message}", ex);
        }
    }

    // SELECT Data by ID
    public async Task<T?> GetByIdAsync(string query, object parameters = null)
    {
        try
        {
            return await Connection.QueryFirstOrDefaultAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing GetByIdAsync: {ex.Message}", ex);
        }
    }

    // SELECT Semua Data
    public async Task<IEnumerable<T>> GetAllAsync(string query, object parameters = null)
    {
        try
        {
            Connection.Open();
            return await Connection.QueryAsync<T>(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing GetAllAsync: {ex.Message}", ex);
        }
    }

    public IEnumerable<T> SelectData(string query)
    {
        try
        {
            Connection.Open();
            if (_connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Koneksi berhasil dibuka!");
            }
            else
            {
                Console.WriteLine($"Status koneksi: {_connection.State}");
            }
            return _connection.Query<T>(query);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing GetAllAsync: {ex.Message}", ex);
        }
    }

    // UPDATE Data
    public async Task<int> UpdateAsync(string query, object parameters = null)
    {
        try
        {
            return await Connection.ExecuteAsync(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing UpdateAsync: {ex.Message}", ex);
        }
    }

    // DELETE Data
    public async Task<int> DeleteAsync(string query, object parameters = null)
    {
        try
        {
            return await Connection.ExecuteAsync(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing DeleteAsync: {ex.Message}", ex);
        }
    }

    // QueryMultiple
    public async Task<(T1?, IEnumerable<T2?>)> QueryMultipleAsync<T1, T2>(string query, object parameters = null)
    {
        try
        {
            using (var multi = await Connection.QueryMultipleAsync(query, parameters))
            {
                var item1 = await multi.ReadFirstOrDefaultAsync<T1>();
                var item2 = (await multi.ReadAsync<T2>()).ToList();
                return (item1, item2);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing QueryMultipleAsync: {ex.Message}", ex);
        }
    }

    // Implement IDisposable
    public void Dispose()
    {
        if (_connection != null && _connection.State != ConnectionState.Closed)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}
