using dba_approved.unit.tests.InternalTestClasses;
using dba_approved_core.DatabaseLayer;
using dba_approved_core.QueryLayer;
using NUnit.Framework;

namespace dba_approved.unit.tests.QueryLayer
{
    [TestFixture]
    public class DatabaseFacadeTests
    {
        [Test]
        public void Can_create_database_facade()
        {
            //This test is to test the syntax of creating a database facade and verify if it's clean
            var database = new DatabaseFacade<ITestConnectionStrings>(new TestConnectionStrings(), connectionStrings => new MsSqlDatabase(connectionStrings.TestConnection));
        }
    }
}
