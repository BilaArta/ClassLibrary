using System.Data;

public interface IDatabaseConnection
{
    public interface IDatabaseConnection
{
    void Connect();
    IEnumerable<T> ExecuteReader<T>(string storedProcedure, Func<IDataReader, T> map, Dictionary<string, object> parameters = null);
    int ExecuteNonQuery(string storedProcedure, Dictionary<string, object> parameters = null);
}

}
