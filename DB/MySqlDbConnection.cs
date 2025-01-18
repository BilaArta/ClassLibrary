using MySql.Data.MySqlClient;
using System.Data;
using ClassLibrary.Helper;

public class MySqlDatabaseConnection : IDatabaseConnection
{
    private readonly string _connectionString;

    public MySqlDatabaseConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect()
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        
        LoggerHelper.Info("Success Connected to Db Mysql");
    }

    public IEnumerable<T> ExecuteReader<T>(string storedProcedure, Func<IDataReader, T> map, Dictionary<string, object> parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using var command = new MySqlCommand(storedProcedure, connection) { CommandType = CommandType.StoredProcedure };
        AddParameters(command, parameters);

        using var reader = command.ExecuteReader();
        var result = new List<T>();
        while (reader.Read())
        {
            result.Add(map(reader));
        }

        return result;
    }

    public int ExecuteNonQuery(string storedProcedure, Dictionary<string, object> parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        using var command = new MySqlCommand(storedProcedure, connection) { CommandType = CommandType.StoredProcedure };
        AddParameters(command, parameters);

        return command.ExecuteNonQuery();
    }

    private void AddParameters(MySqlCommand command, Dictionary<string, object> parameters)
    {
        if (parameters == null) return;
        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Key, param.Value);
        }
    }
}
