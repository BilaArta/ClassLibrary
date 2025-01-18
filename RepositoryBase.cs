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
using MongoDB.Driver.Core.Configuration;
using IsolationLevel = System.Data.IsolationLevel;
using System.Collections.Concurrent;

namespace ClassLibrary.RepositoryBase;
public abstract class BaseRepository<T> : DbContext, IDisposable where T : class
{
    private readonly DatabaseDetail _dbDetailConfig;
    //private IDbConnection? _connection;
    private readonly string setIsolationLevelRead = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
    protected BaseRepository(string DbContext, IConfiguration configuration) : base(DbContext, configuration)
    {
        _dbDetailConfig = GetDatabaseConfig();
    }

    private IDbConnection CreateConnection()
    {
        if (_dbDetailConfig.DatabaseProvider == null)
        {
            throw new InvalidOperationException("Database provider is not configured.");
        }

        return (EnumDatabaseProvider)_dbDetailConfig.DatabaseProvider.Value switch
        {
            EnumDatabaseProvider.SqlServer => new SqlConnection(_dbDetailConfig.ConnectionStrings),
            EnumDatabaseProvider.MySql => new MySqlConnection(_dbDetailConfig.ConnectionStrings),
            EnumDatabaseProvider.Npgsql => new NpgsqlConnection(_dbDetailConfig.ConnectionStrings),
            _ => throw new NotSupportedException($"Database provider {ConnectionName} is not supported.")
        };
    }

    public async Task<List<int>> InsertBatchData<T>(string query, List<T> datas)
    {
        var results = new List<int>();
        using (var connection = CreateConnection())
        {

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Console.WriteLine("Koneksi berhasil dibuka!");
            }

            using (IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    foreach (var data in datas)
                    {
                        // Gunakan ExecuteAsync untuk operasi async
                        var result = await connection.ExecuteAsync(query, data, transaction);
                        results.Add(result);
                    }

                    // Commit transaksi jika berhasil
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback transaksi jika terjadi error
                    transaction.Rollback();
                    Console.WriteLine($"Error during batch insert: {ex.Message}");
                    throw;
                }
            }
        }

        return results;
    }

    public async Task<List<int>> InsertBatchDataParallelAsync<T>(string query, List<T> datas, int batchSize)
    {
        var results = new ConcurrentBag<int>();
        var batchList = datas
            .Select((data, index) => new { data, index })
            .GroupBy(x => x.index / batchSize)
            .Select(g => g.Select(x => x.data).ToList())
            .ToList();

        await Task.WhenAll(batchList.Select(async batch =>
        {
            using (IDbConnection connection = CreateConnection())
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    Console.WriteLine("Koneksi berhasil dibuka!");
                }

                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        foreach (var data in batch)
                        {
                            // Gunakan ExecuteAsync untuk operasi async
                            var result = await connection.ExecuteAsync(query, data, transaction);
                            results.Add(result);
                        }

                        // Commit transaksi untuk batch ini
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaksi jika terjadi error
                        transaction.Rollback();
                        Console.WriteLine($"Error during batch insert: {ex.Message}");
                        throw;
                    }
                }
            }
        }));

        return results.ToList();
    }


    // INSERT Data
    public async Task<int> InsertAsyncTransaction(string query, object parameters)
    {
        try
        {
            using (var connection = CreateConnection())
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    try
                    {
                        var result = await connection.ExecuteAsync(query, parameters, transaction);
                        LoggerHelper.Info($"Data created :{result.ToString()}");
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        LoggerHelper.Error($"InsertAsyncTransaction::Exception Error:{e}");
                        throw new Exception($"Error executing InsertAsync: {e.Message}", e);
                    }
                }
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
            return await CreateConnection().ExecuteAsync(query, parameters);
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
            return await CreateConnection().QueryFirstOrDefaultAsync<T>(query, parameters);
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
            using (var connection = CreateConnection())
            {
                //connection.Open();
                return await connection.QueryAsync<T>(query, parameters);
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"Error Exception::GetAllAsync::Ex Error :{ex}");
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }

    }

    public IEnumerable<T> SelectData(string query)
    {
        try
        {
            using (var connection = CreateConnection())
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    Console.WriteLine("Koneksi berhasil dibuka!");
                }
                else
                {
                    Console.WriteLine($"Status koneksi: {connection.State}");
                }

                return connection.Query<T>(query);
            }
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
            return await CreateConnection().ExecuteAsync(query, parameters);
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
            return await CreateConnection().ExecuteAsync(query, parameters);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing DeleteAsync: {ex.Message}", ex);
        }
    }

    // QueryMultiplea
    public async Task<(T1?, IEnumerable<T2?>)> QueryMultipleAsync<T1, T2>(string query, object parameters = null)
    {
        try
        {
            using (var multi = await CreateConnection().QueryMultipleAsync(query, parameters))
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
    }
}
