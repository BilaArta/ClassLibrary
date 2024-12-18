using Cassandra;
using System;


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

                Console.WriteLine("Success Connected to Cassandra.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed Connect to Cassandra: {ex.Message}");
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
    }
}