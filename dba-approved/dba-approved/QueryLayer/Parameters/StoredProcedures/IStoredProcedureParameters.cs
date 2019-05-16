using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved_core.QueryLayer.Parameters.StoredProcedures
{
    /// <summary>
    /// IStoredProcedureParameters, when used with "AddStoredProcedureParameters" in QueryParameterExtensions,
    /// allows you to specify properties and methods on an object that should map to specific stored procedures.
    /// </summary>
    public interface IStoredProcedureParameters
    {
        /// <summary>
        /// Get Defaults allows you to create an enumerable of defaulted values per stored procedure in case one
        /// stored procedure on a re-used object member should default to "true" while another should default to "false".
        ///
        /// There is an in-line DefaultValueIfNull property on StoredProcedureParameterAttribute, so simple data types such as bool and int do not
        /// need to be specified here, but if you were to default a Guid or DateTime, or some other complex type, you could add it here.
        ///
        /// Important to Note: Anything assigned here will be overridden by any in-line defaultValues.
        /// </summary>
        /// <returns>An enumerable of objects that help determine the default value for a null member</returns>
        IEnumerable<DefaultStoredProcedureParameterValue> GetDefaults();
    }
}
