using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

public abstract class BaseRepository<T> : IDisposable where T : class
{
    private readonly string _connectionString;
    private readonly DatabaseType _databaseType;
    private IDbConnection _connection;

    protected BaseRepository(string connectionString, DatabaseType databaseType)
    {
        _connectionString = connectionString;
        _databaseType = databaseType;
    }

    protected IDbConnection Connection
    {
        get
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = _databaseType switch
                {
                    DatabaseType.SqlServer => new SqlConnection(_connectionString),
                    DatabaseType.MySql => new MySqlConnection(_connectionString),
                    _ => throw new NotSupportedException($"Database type {_databaseType} is not supported.")
                };
            }
            return _connection;
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
    public async Task<T> GetByIdAsync(string query, object parameters = null)
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
            return await Connection.QueryAsync<T>(query, parameters);
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
    public async Task<(T1, IEnumerable<T2>)> QueryMultipleAsync<T1, T2>(string query, object parameters = null)
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
