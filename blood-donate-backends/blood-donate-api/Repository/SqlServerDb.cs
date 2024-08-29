using System.Data;
using System.Data.SqlClient;

namespace blood_donate_api.Repository
{
    public class SqlServerDb : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;

        public SqlServerDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<SqlConnection> GetConnectionAsync()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
               await _connection.OpenAsync();
            }
            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
            GC.SuppressFinalize(this);
        }
    }
}
