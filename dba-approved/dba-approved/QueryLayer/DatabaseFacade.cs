using dba_approved_core.DatabaseLayer;
using dba_approved_core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace dba_approved_core.QueryLayer
{
    /// <summary>
    /// Facade-pattern database connection wrapper for easily executing queries and stored procedures, and returning parsed data.
    /// </summary>
    public class DatabaseFacade<TConnectionStrings> : IQueryableDatabaseFacade, IProceduralDatabaseFacade where TConnectionStrings : IDatabaseConnectionStrings
    {
        #region constructors
        private readonly IDatabase _database;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStrings">ConnectionStrings Object with predefined getters to return connection strings</param>
        /// <param name="getDatabase">Function that takes in ConnectionStrings Object and returns a specified connectionString based on that object</param>
        public DatabaseFacade(TConnectionStrings connectionStrings, Func<TConnectionStrings, IDatabase> getDatabase)
        {
           
            _database = getDatabase(connectionStrings);

        }

        /// <summary>
        /// Create a DatabaseFacade using an existing SqlConnection
        /// </summary>
        /// <param name="connectionPoolEntry"></param>
        public DatabaseFacade(SqlConnection connectionPoolEntry, Func<SqlConnection, IDatabase> getDatabase)
        {
            _database = getDatabase(connectionPoolEntry);
        }
        #endregion

        #region IQueryableDatabaseFacade Methods
        public T FirstOrDefault<T>(string sql, Action<SqlCommand> parameterize, Func<DataTable, T> parse)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                if (table.Rows.Count == 0)
                    return default(T);

                return parse(table);
            });
        }
        public T FirstOrDefaultFromEnumerable<T>(string sql, Action<SqlCommand> parameterize, Func<DataRow, T> parse) =>
            FetchFromEnumerable(sql, parameterize, parse).FirstOrDefault();

        public T First<T>(string sql, Action<SqlCommand> parameterize, Func<DataTable, T> parse)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                if (table.Rows.Count == 0)
                    throw new InvalidOperationException("Sequence contains no elements");

                return parse(table);
            });
        }

        public T FirstFromEnumerable<T>(string sql, Action<SqlCommand> parameterize, Func<DataRow, T> parse) =>
            FetchFromEnumerable(sql, parameterize, parse).First();

        /// <summary>
        /// Fetches a List from the result set of a query.
        ///
        /// Takes a datatable and parses data to create a list.
        /// </summary>
        /// <param name="sql">The Query to run.</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take a datatable and return a list.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A list from the result set of a query.</returns>
        public IList<T> Fetch<T>(string sql, Action<SqlCommand> parameterize, Func<DataTable, IList<T>> parse)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                return parse(table);
            });
        }

        /// <summary>
        /// Fetches a List from the result set of a query.
        ///
        /// Takes a datarow and parses a single instanceof(T). Then it returns the IEnumerable(T) as a List(T).
        /// Because parsing at the row-level, this should only be used when each line of data is a unique entry.
        /// </summary>
        /// <param name="sql">The Query to run.</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take in a dataRow and return a single instanceof(T) the list.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A list from the result set of a query.</returns>
        public IList<T> FetchFromEnumerable<T>(string sql, Action<SqlCommand> parameterize, Func<DataRow, T> parse)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                return FetchEnumerableFromDataTable(table, parse).ToList();
            });
        }

        /// <summary>
        /// Fetches a Dictionary from the result set of a query.
        ///
        /// Takes a datatable and parses data to create a dictionary.
        /// </summary>
        /// <param name="sql">The Query to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take a datatable and return a dictionary.</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>A dictionary from the result set of a query.</returns>
        public IDictionary<TKey, TValue> FetchDictionary<TKey, TValue>(string sql, Action<SqlCommand> parameterize, Func<DataTable, IDictionary<TKey, TValue>> parse)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                return parse(table);
            });
        }

        /// <summary>
        /// Fetches a Dictionary from the result set of a query.
        ///
        /// Takes a datarow and parses a Key-Value pair. Then it turns the list of Key-Value pairs into a Dictionary.
        /// Because parsing at the row-level, this should only be used when each line of data is a unique entry.
        /// </summary>
        /// <param name="sql">The Query to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take in a dataRow and return a Key-Value Pair which will be used to build the dictionary.</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>A dictionary from the result set of a query.</returns>
        public IDictionary<TKey, TValue> FetchDictionaryFromEnumerable<TKey, TValue>(string sql, Action<SqlCommand> parameterize, Func<DataRow, KeyValuePair<TKey, TValue>> parse)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }

                return FetchEnumerableFromDataTable(table, parse).ToDictionary(x => x.Key, x => x.Value);
            });
        }

        /// <summary>
        /// Executes Inline-Sql. Returns true if sql has run. If you wish to evaluate success based on
        /// rowsAffected or some other return value, run the alternative Execute function with a Typed return value.
        /// </summary>
        /// <param name="sql">The Stored Procedure to run.</param>
        /// <param name="parameterize">
        /// The function to assign Parameters to a SqlCommand.
        /// </param>
        /// <returns>True if success. Throws SqlException if failure occurs.</returns>
        public bool Execute(string sql, Action<SqlCommand> parameterize)
        {
            return ExecuteSql(sql, parameterize);
        }

        /// <summary>
        /// Executes Inline-Sql and returns the out-value in the format requested.
        ///
        /// e.g. If you run a "Create Record" and the "Create Record" command returns an auto-generated id,
        /// run this and return an int or long value to continue processing using that value.
        /// </summary>
        /// <param name="sql">The Inline-Sql to run.</param>
        /// <param name="parameterize">
        /// The function to assign Parameters to a SqlCommand.
        /// </param>
        /// <typeparam name="T">Return type with which to cast the returned value.</typeparam>
        /// <returns>The value returned from the stored procedure, cast as requested Type.</returns>
        public T Execute<T>(string sql, Action<SqlCommand> parameterize)
        {
            return WrapQuery(sql, command =>
            {
                parameterize?.Invoke(command);
                var returnValue = command.ExecuteScalar();
                return RuntimeParsing.GetParserMethod<T>()(returnValue.ToString());
            });

        }

        #endregion

        #region IProceduralDatabaseFacade Methods
        public T FirstFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize,
    Func<DataTable, T> parse)
        {
            return First(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            }, parse);
        }

        public T FirstFromEnumerableFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataRow, T> parse) =>
            FetchFromEnumerableFromProcedure(procedureName, parameterize, parse).FirstOrDefault();



        public T FirstOrDefaultFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize,
            Func<DataTable, T> parse)
        {
            return FirstOrDefault(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            }, parse);
        }

        /// <summary>
        /// Fetches the first value the result set of a procedure.
        ///
        /// Takes the first datarow and parses a single instanceof(T).
        /// This can be used easily pull back a single value from a procedure that returns multiple columns, to enhance procedure re-useability.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take in a dataRow and return a single instanceof(T).</param>
        /// <typeparam name="T">T must be a struct for this to compile. This is to help return default(T) as the </typeparam>
        /// <returns>The first selected value from the result set of a procedure</returns>
        public T FirstOrDefaultFromEnumerableFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize,
            Func<DataRow, T> parse) =>
            FetchFromEnumerableFromProcedure(procedureName, parameterize, parse).FirstOrDefault();





        /// <summary>
        /// Fetches a List from the result set of a procedure.
        ///
        /// Takes a datatable and parses data to create a list.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take a datatable and return a list.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A list from the result set of a procedure.</returns>
        public IList<T> FetchFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataTable, IList<T>> parse)
        {
            return Fetch(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            }, parse);
        }


        /// <summary>
        /// Fetches a List from the result set of a procedure.
        ///
        /// Takes a datarow and parses a single instanceof(T). Then it returns the IEnumerable(T) as a List(T).
        /// Because parsing at the row-level, this should only be used when each line of data is a unique entry.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take in a dataRow and return a single instanceof(T) to the list.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A list from the result set of a procedure</returns>
        public IList<T> FetchFromEnumerableFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataRow, T> parse)
        {
            return FetchFromEnumerable(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            }, parse);
        }



        /// <summary>
        /// Fetches a Dictionary from the result set of a procedure.
        ///
        /// Takes a datatable and parses data to create a dictionary.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take a datatable and return a dictionary.</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>A dictionary from the result set of a procedure.</returns>
        public IDictionary<TKey, TValue> FetchDictionaryFromProcedure<TKey, TValue>(string procedureName,
            Action<SqlCommand> parameterize, Func<DataTable, IDictionary<TKey, TValue>> parse)
        {
            return FetchDictionary(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            }, parse);
        }


        /// <summary>
        /// Fetches a Dictionary from the result set of a procedure.
        ///
        /// Takes a datarow and parses a Key-Value pair. Then it turns the list of Key-Value pairs into a Dictionary.
        /// Because parsing at the row-level, this should only be used when each line of data is a unique entry.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take in a dataRow and return a Key-Value Pair which will be used to build the dictionary.</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>A dictionary from the result set of a procedure.</returns>
        public IDictionary<TKey, TValue> FetchDictionaryFromEnumerableFromProcedure<TKey, TValue>(string procedureName,
            Action<SqlCommand> parameterize, Func<DataRow, KeyValuePair<TKey, TValue>> parse)
        {
            return FetchDictionaryFromEnumerable(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            }, parse);
        }

        /// <summary>
        /// Executes Procedure. Returns true if stored procedure has run.
        /// This is because Stored Procedures don't have a default way to verify success.
        ///
        /// e.g. Both a successful and non-successful stored procedure execution might return "0", but you could have
        /// procedures that return a success or failure value. In those situations you would use the alternative
        /// ExecuteProcedure method to return the out-value in the format requested.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run.</param>
        /// <param name="parameterize">
        /// The function to assign Parameters to a SqlCommand.
        /// 
        /// It is highly recommended to create an interoperability object and decorate the properties and methods with StoredProcedureParameterAttribute.
        /// If you do this, make sure to apply AddStoredProcedureParameters instead of AddParameters to ensure you supply the necessary parameters only.
        /// </param>
        /// <returns>True if success. Throws SqlException if failure occurs.</returns>
        public bool ExecuteProcedure(string procedureName, Action<SqlCommand> parameterize)
        {
            return ExecuteSql(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            });
        }
        /// <summary>
        /// Executes Stored Procedure and returns the out-value in the format requested.
        ///
        /// e.g. If you run a "Create Record" and the "Create Record" command returns an auto-generated id,
        /// run this and return an int or long value to continue processing using that value.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run.</param>
        /// <param name="parameterize">
        /// The function to assign Parameters to a SqlCommand.
        /// 
        /// It is highly recommended to create an interoperability object and decorate the properties and methods with StoredProcedureParameterAttribute.
        /// If you do this, make sure to apply AddStoredProcedureParameters instead of AddParameters to ensure you supply the necessary parameters only.
        /// </param>
        /// <typeparam name="T">Return type with which to cast the returned value.</typeparam>
        /// <returns>The value returned from the stored procedure, cast as requested Type.</returns>
        public T ExecuteProcedure<T>(string procedureName, Action<SqlCommand> parameterize)
        {
            return Execute<T>(procedureName, command =>
            {
                command.CommandType = CommandType.StoredProcedure;
                parameterize?.Invoke(command);
            });
        }
        #endregion

        #region Internal Database Access Methods
        private bool ExecuteSql(string sql, Action<SqlCommand> parameterize) =>
            _database.ExecuteSql(sql, parameterize);
       
        private T WrapQuery<T>(string sql, Func<SqlCommand, T> executeOperation) =>
            _database.WrapQuery<T>(sql, executeOperation);
  
        private IEnumerable<T> FetchEnumerableFromDataTable<T>(DataTable table, Func<DataRow, T> parse) =>
            _database.FetchEnumerableFromDataTable(table, parse);
        
        #endregion
    }
}
