using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace dba_approved_core.QueryLayer
{
    /// <summary>
    /// Interface for Facade-pattern database connection wrapper for easily executing stored procedures and returning parsed data.
    /// </summary>
    public interface IProceduralDatabaseFacade
    {
        T FirstOrDefaultFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize,
            Func<DataTable, T> parse);

        T FirstOrDefaultFromEnumerableFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize,  Func<DataRow, T> parse);

        T FirstFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataTable, T> parse);

        T FirstFromEnumerableFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataRow, T> parse);

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
        IList<T> FetchFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataTable, IList<T>> parse);

        /// <summary>
        /// Fetches a List from the result set of a procedure.
        ///
        /// Takes a datarow and parses a single instanceof(T). Then it returns the IEnumerable(T) as a List(T).
        /// Because parsing at the row-level, this should only be used when each line of data is a unique entry.
        /// </summary>
        /// <param name="procedureName">The Stored Procedure to run</param>
        /// <param name="parameterize">The function to assign Parameters to a SqlCommand.</param>
        /// <param name="parse">The function to take in a dataRow and return a single instanceof(T) the list.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>A list from the result set of a procedure</returns>
        IList<T> FetchFromEnumerableFromProcedure<T>(string procedureName, Action<SqlCommand> parameterize, Func<DataRow, T> parse);

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
        IDictionary<TKey, TValue> FetchDictionaryFromProcedure<TKey, TValue>(string procedureName,
            Action<SqlCommand> parameterize, Func<DataTable, IDictionary<TKey, TValue>> parse);

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
        IDictionary<TKey, TValue> FetchDictionaryFromEnumerableFromProcedure<TKey, TValue>(string procedureName,
            Action<SqlCommand> parameterize, Func<DataRow, KeyValuePair<TKey, TValue>> parse);

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
        bool ExecuteProcedure(string procedureName, Action<SqlCommand> parameterize);

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
        T ExecuteProcedure<T>(string procedureName, Action<SqlCommand> parameterize);
    }
}
