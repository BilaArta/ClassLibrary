using Cassandra;
using System;
using ClassLibrary.Helper;

namespace ClassLibrary.DB
{
    public class CassandraConnection : IDisposable
    {
        private readonly Cluster _cluster;
        private ISession _session;

        /// <summary>
        /// Constructor untuk menginisialisasi koneksi ke Cassandra.
        /// </summary>
        /// <param name="contactPoints">Array alamat IP atau host dari cluster Cassandra.</param>
        /// <param name="keyspace">Keyspace yang akan digunakan.</param>
        public CassandraConnection(string[] contactPoints, string keyspace)
        {
            if (contactPoints == null || contactPoints.Length == 0)
            {
                LoggerHelper.Error($"Contact points empty. {nameof(contactPoints)}");
                throw new ArgumentException("Contact points empty.", nameof(contactPoints));
            }

            try
            {
                // Membuat cluster
                _cluster = Cluster.Builder()
                    .AddContactPoints(contactPoints)
                    .Build();

                // Membuka sesi
                _session = _cluster.Connect();

                // Set keyspace jika disediakan
                if (!string.IsNullOrEmpty(keyspace))
                {
                    _session.ChangeKeyspace(keyspace);
                }
                LoggerHelper.Info("Success Connected to Cassandra.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error($"Failed Connect to Cassandra: {ex.Message}");
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Mendapatkan sesi aktif ke Cassandra.
        /// </summary>
        public ISession GetSession()
        {
            if (_session == null)
            {
                LoggerHelper.Error("Connection Host Empty.");
                throw new InvalidOperationException("Connection Host Empty.");
            }

            return _session;
        }

        /// <summary>
        /// Menutup koneksi dan membersihkan sumber daya.
        /// </summary>
        public void Dispose()
        {
            _session?.Dispose();
            _cluster?.Dispose();
        }

        public void Test(){
        var cluster = Cluster.Builder()
                             .AddContactPoint("127.0.0.1") // Ganti dengan IP/hostname ScyllaDB
                             .WithPort(9042)
                             .Build();

            using (var session = cluster.Connect())
            {
                session.CreateKeyspaceIfNotExists("example", 
                    new Dictionary<string, string> { { "class", "SimpleStrategy" }, { "replication_factor", "1" } });

                session.ChangeKeyspace("example");
                session.Execute("CREATE TABLE IF NOT EXISTS users (id UUID PRIMARY KEY, name text, age int)");
                session.Execute("INSERT INTO users (id, name, age) VALUES (uuid(), 'Alice', 30)");

                var result = session.Execute("SELECT * FROM users");
                foreach (var row in result)
                {
                    Console.WriteLine($"ID: {row["id"]}, Name: {row["name"]}, Age: {row["age"]}");
                }
            }
        }
    }
}