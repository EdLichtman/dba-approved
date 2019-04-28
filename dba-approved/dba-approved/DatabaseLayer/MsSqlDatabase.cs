using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace dba_approved_core.DatabaseLayer
{
    public class MsSqlDatabase : IDatabase
    {
        private readonly SqlConnection _connectionPoolEntry;
        private readonly string _connectionString;
        public MsSqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
            _connectionPoolEntry = null;
        }

        protected MsSqlDatabase(SqlConnection connectionPoolEntry)
        {
            _connectionPoolEntry = connectionPoolEntry;
        }

        public bool ExecuteSql(string sql, Action<SqlCommand> parameterize)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                command.ExecuteNonQuery();
                return true;
            });
        }

        public T WrapQuery<T>(string sql, Func<SqlCommand, T> executeOperation)
        {
            if (_connectionPoolEntry != null)
            {
                using (var command = new SqlCommand(sql, _connectionPoolEntry))
                {
                    return executeOperation(command);
                }
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        return executeOperation(command);
                    }
                }
            }
        }

        public IEnumerable<T> FetchEnumerableFromDataTable<T>(DataTable table, Func<DataRow, T> parse)
        {
            foreach (DataRow tableRow in table.Rows)
            {
                yield return parse(tableRow);
            }
        }
    }
}
