using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace dba_approved_core.QueryLayer
{
    /// <summary>
    /// Interface for Facade-pattern database connection wrapper for easily executing queries and returning parsed data.
    /// </summary>
    public interface IQueryableDatabaseFacade
    {
        T First<T>(string sql, Action<SqlCommand> parameterize, Func<DataTable, T> parse);
        T FirstFromEnumerable<T>(string sql, Action<SqlCommand> parameterize, Func<DataRow, T> parse);

        T FirstOrDefault<T>(string sql, Action<SqlCommand> parameterize, Func<DataTable, T> parse);
        T FirstOrDefaultFromEnumerable<T>(string sql, Action<SqlCommand> parameterize, Func<DataRow, T> parse);


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
        IList<T> Fetch<T>(string sql, Action<SqlCommand> parameterize, Func<DataTable, IList<T>> parse);

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
        IList<T> FetchFromEnumerable<T>(string sql, Action<SqlCommand> parameterize, Func<DataRow, T> parse);

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
        IDictionary<TKey, TValue> FetchDictionary<TKey, TValue>(string sql, Action<SqlCommand> parameterize, Func<DataTable, IDictionary<TKey, TValue>> parse);

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
        IDictionary<TKey, TValue> FetchDictionaryFromEnumerable<TKey, TValue>(string sql, Action<SqlCommand> parameterize, Func<DataRow, KeyValuePair<TKey, TValue>> parse);



        /// <summary>
        /// Executes Inline-Sql. Returns true if sql has run. If you wish to evaluate success based on
        /// rowsAffected or some other return value, run the alternative Execute function with a Typed return value.
        /// </summary>
        /// <param name="sql">The Stored Procedure to run.</param>
        /// <param name="parameterize">
        /// The function to assign Parameters to a SqlCommand.
        /// </param>
        /// <returns>True if success. Throws SqlException if failure occurs.</returns>
        bool Execute(string sql, Action<SqlCommand> parameterize);

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
        T Execute<T>(string sql, Action<SqlCommand> parameterize);
    }
}
