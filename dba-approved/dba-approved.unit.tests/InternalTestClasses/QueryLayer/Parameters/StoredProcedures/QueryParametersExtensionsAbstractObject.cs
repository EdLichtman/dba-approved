using dba_approved_core.QueryLayer.Parameters.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved.unit.tests.InternalTestClasses.QueryLayer.Parameters.StoredProcedures
{
    [StoredProcedure(GlobalStoredProcedureName)]
    internal class QueryParametersExtensionsAbstractTestObject : IStoredProcedureParameters
    {
        public const string GlobalStoredProcedureName = "testProc";
        public const string StoredProcedureAlias = "fakeProp";
        public const string StoredProcedureAliasIgnoreNull = "ignoreNull";
        public const string StoredProcedureMethodAlias = "methodProperty";
        public const int StoredProcedurePropertyNullDefaultConstantValue = 12345;
        public static readonly Guid StoredProcedureComplexPropertyNullDefaultValue = Guid.NewGuid();

        public const string DuplicateParameterName1 = "duplicateParameterName1";
        public const string DuplicateParameterName2 = "duplicateParameterName2";

        [StoredProcedureParameter(StoredProcedureAlias)]
        public string AliasableProperty { get; set; }
        public string ExclusionProperty { get; set; }

        [StoredProcedureParameter(StoredProcedureAliasIgnoreNull, IgnoreIfNull = true)]
        public string IgnoreIfNullProperty { get; set; }

        [StoredProcedureParameter(StoredProcedureMethodAlias)]
        public int GetAliasedMethodProperty() => 1;

        [StoredProcedureParameter(nameof(GetMethodProperty))]
        public int GetMethodProperty() => 2;

        [StoredProcedureParameter(nameof(TestDefaultedProperty), DefaultValueIfNull = StoredProcedurePropertyNullDefaultConstantValue)]
        public int? TestDefaultedProperty { get; set; }

        [StoredProcedureParameter(nameof(TestComplexDefaultedProperty))]
        public Guid? TestComplexDefaultedProperty { get; set; }

        [StoredProcedureParameter(DuplicateParameterName1)]
        [StoredProcedureParameter(DuplicateParameterName2)]
        public Guid DuplicateProperty { get; set; } = Guid.NewGuid();

        public IEnumerable<DefaultStoredProcedureParameterValue> GetDefaults()
        {
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(TestComplexDefaultedProperty),
                DefaultValue = StoredProcedureComplexPropertyNullDefaultValue
            };
        }
    }
}
