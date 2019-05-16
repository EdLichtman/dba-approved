using dba_approved_core.QueryLayer.Parameters.StoredProcedures;
using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved.unit.tests.InternalTestClasses.QueryLayer.Parameters.StoredProcedures
{
    [StoredProcedure(GlobalStoredProcedureName)]
    internal class ExtraLargeQueryParametersExtensionsAbstractGlobalTestObject : IStoredProcedureParameters
    {
        public const string GlobalStoredProcedureName = "testProc";
        public const int StoredProcedurePropertyNullDefaultConstantValue = 12345;
        public static readonly Guid StoredProcedureComplexPropertyNullDefaultValue = Guid.NewGuid();



        [StoredProcedureParameter(nameof(ParameterizableProperty))]
        public string ParameterizableProperty { get; set; }

        [StoredProcedureParameter(nameof(Property1))]
        public string Property1 { get; set; }

        [StoredProcedureParameter(nameof(Property2))]
        public string Property2 { get; set; }

        [StoredProcedureParameter(nameof(Property3), DefaultValueIfNull = int.MaxValue)]
        public int? Property3 { get; set; }

        [StoredProcedureParameter(nameof(Property4))]
        public DateTime? Property4 { get; set; }

        [StoredProcedureParameter(nameof(Property5), DefaultValueIfNull = long.MaxValue)]
        public long? Property5 { get; set; }

        [StoredProcedureParameter(nameof(Property6))]
        public decimal? Property6 { get; set; }

        [StoredProcedureParameter(nameof(Property7))]
        public string Property7 { get; set; } = "foo";

        [StoredProcedureParameter(nameof(Property8))]
        public string Property8 { get; set; } = "bar";

        [StoredProcedureParameter(nameof(Property9))]
        public string Property9 { get; set; } = "don't";

        [StoredProcedureParameter(nameof(Property10))]
        public string Property10 { get; set; } = "panic";

        [StoredProcedureParameter(nameof(Property11))]
        public string Property11 { get; set; }

        [StoredProcedureParameter(nameof(Property12))]
        public string Property12 { get; set; }
        [StoredProcedureParameter(nameof(Property13))]
        public string Property13 { get; set; }

        [StoredProcedureParameter(nameof(Property14))]
        public string Property14 { get; set; }

        [StoredProcedureParameter(nameof(Property15))]
        public string Property15 { get; set; }

        [StoredProcedureParameter(nameof(Property16))]
        public string Property16 { get; set; }
        [StoredProcedureParameter(nameof(Property17))]
        public string Property17 { get; set; }

        [StoredProcedureParameter(nameof(Property18))]
        public string Property18 { get; set; }

        [StoredProcedureParameter(nameof(Property19))]
        public long? Property19 { get; set; }
        [StoredProcedureParameter(nameof(Property20))]
        public Guid? Property20 { get; set; }

        [StoredProcedureParameter(nameof(Property21))]
        public string Property21 { get; set; } = "foo";

        [StoredProcedureParameter(nameof(Property22))]
        public string Property22 { get; set; } = "bar";

        [StoredProcedureParameter(nameof(Property23))]
        public string Property23 { get; set; } = "don't";

        [StoredProcedureParameter(nameof(Property24))]
        public string Property24 { get; set; } = "panic";

        [StoredProcedureParameter(nameof(Property25))]
        public string Property25 { get; set; } = "foo";

        [StoredProcedureParameter(nameof(Property26))]
        public string Property26 { get; set; } = "bar";

        [StoredProcedureParameter(nameof(Property27))]
        public string Property27 { get; set; } = "don't";

        [StoredProcedureParameter(nameof(Property28))]
        public string Property28 { get; set; } = "panic";

        [StoredProcedureParameter(nameof(Property29))]
        public string Property29 { get; set; } = "don't";

        [StoredProcedureParameter(nameof(Property30))]
        public string Property30 { get; set; } = "panic";
        public string ExclusionProperty { get; set; }
        [StoredProcedureParameter(nameof(IgnoreIfNullProperty), IgnoreIfNull = true)]
        public string IgnoreIfNullProperty { get; set; }

        [StoredProcedureParameter(nameof(Method1))]
        public int Method1() => 1;

        [StoredProcedureParameter(nameof(GetMethodProperty))]
        public int GetMethodProperty() => 2;

        [StoredProcedureParameter(nameof(TestDefaultedProperty), DefaultValueIfNull = StoredProcedurePropertyNullDefaultConstantValue)]
        public int? TestDefaultedProperty { get; set; }

        [StoredProcedureParameter(nameof(TestComplexDefaultedProperty))]
        public Guid? TestComplexDefaultedProperty { get; set; }

        [StoredProcedureParameter("DuplicateProperty1")]
        [StoredProcedureParameter("DuplicateProperty2")]
        public Guid DuplicateProperty { get; set; } = Guid.NewGuid();

        public IEnumerable<DefaultStoredProcedureParameterValue> GetDefaults()
        {
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(TestComplexDefaultedProperty),
                DefaultValue = StoredProcedureComplexPropertyNullDefaultValue
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property4),
                DefaultValue = DateTime.Now
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property6),
                DefaultValue = 3.7m
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property11),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property12),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property13),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property14),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property15),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property16),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property17),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property18),
                DefaultValue = Guid.NewGuid().GetHashCode().ToString()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property19),
                DefaultValue = Guid.NewGuid().GetHashCode()
            };
            yield return new DefaultStoredProcedureParameterValue
            {
                StoredProcedureName = GlobalStoredProcedureName,
                MemberName = nameof(Property20),
                DefaultValue = Guid.NewGuid()
            };
        }
    }
}
