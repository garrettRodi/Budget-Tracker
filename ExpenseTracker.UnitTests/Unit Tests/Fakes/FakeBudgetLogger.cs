using BudgetTracker.Infrastructure.Interface;
using System;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    public class FakeBudgetLogger : IBudgetLogger
    {
        public void Log(string message)
        {
            // For testing, we can simply do nothing or log to the test output.
        }
    }
}
