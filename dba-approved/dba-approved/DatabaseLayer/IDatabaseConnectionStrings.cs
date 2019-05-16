using System;
using System.Collections.Generic;
using System.Text;

namespace dba_approved_core.DatabaseLayer
{
    /// <summary>
    /// IConnectionStrings, though it has no required properties, enforces a style of abstraction to allow your code to be cleaner and testable. 
    /// Instead of using ConfigurationManager to retrieve connection strings, create your own interface (IConnectionStrings) and concrete implementation (ConnectionStrings).
    /// </summary>
    public interface IDatabaseConnectionStrings
    {
    }
}
