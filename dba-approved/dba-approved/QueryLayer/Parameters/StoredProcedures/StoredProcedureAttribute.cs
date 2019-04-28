using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved_core.QueryLayer.Parameters.StoredProcedures
{
    /// <summary>
    /// Stored Procedure allows us to specify a global stored procedure for a StoredProcedureParameters object.
    ///
    /// When a global stored procedure is declared, any parameter that does not declare a stored procedure name uses the global stored procedure name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class StoredProcedureAttribute : Attribute
    {
        public string StoredProcedureName { get; }

        public StoredProcedureAttribute(string storedProcedureName)
        {
            StoredProcedureName = storedProcedureName;
        }
    }
}
