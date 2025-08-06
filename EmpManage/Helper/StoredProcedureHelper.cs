using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;

namespace EmpManage.Helper
{
    public class StoredProcedureHelper
    {
        private readonly string? _connectionString;
        public StoredProcedureHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Connection");
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(string procedureName, Func<SqlDataReader, T> mapFunc, Dictionary<string, object>? parameters = null)
        {
            var results = new List<T>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(procedureName, conn)) 
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null) {
                    foreach (var param in parameters) { 
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                await conn.OpenAsync();
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(mapFunc(reader));
                }
            }

            return results;
        
    }

    }
}
