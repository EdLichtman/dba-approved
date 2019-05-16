using System;
namespace dba_approved_core.QueryLayer.Parsing
{
    /// <summary>
    /// Column Name allows us to map an entry from a DataRow to a property on an object.
    /// It is to be used with DatabaseFacade. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnNameAttribute : Attribute
    {
        /// <summary>
        /// The name of the column to ensure a contract with the database is met, in case refactoring occurs.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Assigning a column name requires is required to ensure a contract is met with the database.
        /// </summary>
        /// <param name="columnName"></param>
        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }

    }
}
   