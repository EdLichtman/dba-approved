using dba_approved_core.DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved.unit.tests.InternalTestClasses
{
    internal interface ITestConnectionStrings : IDatabaseConnectionStrings
    {
        string TestConnection { get; }
    }
    internal class TestConnectionStrings : ITestConnectionStrings
    {
        public string TestConnection => "test";
    }
    
}
