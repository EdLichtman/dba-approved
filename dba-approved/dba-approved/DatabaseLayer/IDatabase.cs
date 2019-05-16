using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace dba_approved_core.DatabaseLayer
{
    public interface IDatabase
    {
        bool ExecuteSql(string sql, Action<SqlCommand> parameterize);
        T WrapQuery<T>(string sql, Func<SqlCommand, T> executeOperation);
        IEnumerable<T> FetchEnumerableFromDataTable<T>(DataTable table, Func<DataRow, T> parse);
    }
}