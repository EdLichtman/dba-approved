using dba_approved_core.QueryLayer.Parsing;
namespace dba_approved.unit.tests.InternalTestClasses
{
    internal class TestDto
    {
        [ColumnName("test_Property")]
        public string TestProperty { get;set; }
    }
}
