using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved_core.QueryLayer.Parameters.StoredProcedures
{
    /// <summary>
    /// Stored Procedure Parameter allows us to specify a class property or method as a Stored Procedure Parameter to allow for cleaner interoperability with stored procedures.
    ///
    /// This must be called with StoredProcedure name. If no parameterName is used, the name of the parameter or method is to be used instead.
    /// i.e. If our business object has property TransactionId but a specific Stored Procedure uses "txnId", we can optionally label the parameterName so do not have to separately
    /// declare these properties when calling "command.AddParameters&lt;T&gt;()"
    ///
    /// This Attribute can be used multiple times on a Property or Method to allow for multiple StoredProcedures to access it, resulting in a more re-useable POCO object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class StoredProcedureParameterAttribute : Attribute
    {
        /// <summary>
        /// The alias of this mapping property
        /// </summary>
        public string Alias { get;set; }
        /// <summary>
        /// If Property or Method returns a null value, this tag specifies it should not be added to specific stored procedure.
        /// </summary>
        public bool IgnoreIfNull { get; set; }
        /// <summary>
        /// To allow for multiple stored procedures re-using the same field, we need to allow for multiple default values depending on the Stored Procedure Type.
        /// This replaces an instance of null with the defaulted value.
        ///
        /// In-line defaults must be of a constant struct type, i.e. int, string, double, etc. However, for DateTimes, Guids, or any other non-constant type,
        /// you can use the GetDefaults method on IStoredProcedureParameters. This takes precedence over the results of GetDefaults.
        /// </summary>
        public object DefaultValueIfNull { get; set; }
        /// <summary>
        /// Creation of StoredProcedure Alias mapping requires parameter alias
        /// </summary>
        /// <param name="storedProcedureParameterAlias"></param>
        public StoredProcedureParameterAttribute(string storedProcedureParameterAlias)
        {
            Alias = storedProcedureParameterAlias;
        }

    }
}
