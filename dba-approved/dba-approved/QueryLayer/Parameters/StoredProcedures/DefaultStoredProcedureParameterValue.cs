using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved_core.QueryLayer.Parameters.StoredProcedures
{
    /// <summary>
    /// The requirements to determine the default value for a complex null member.
    /// This is used in IStoredProcedureParameters.GetDefaults()
    ///
    /// e.g if DateTime should default to 1900 because Sql does not accept C# DateTime.Min, you could specify it here.
    /// </summary>
    public class DefaultStoredProcedureParameterValue
    {
        /// <summary>
        /// The stored procedure to which the default applies.
        /// </summary>
        public string StoredProcedureName { get; set; }

        /// <summary>
        /// The Object Member to which the default applies.
        ///
        /// Important to note: This only works off the member (i.e. PropertyName or MethodName), not the Sql Alias that it is assigned to.
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// The Default Value to assign a member if that member is null.
        /// </summary>
        public object DefaultValue { get; set; }
    }
}
