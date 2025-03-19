using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Exceptions
{
    public class BudgetTrackerException : Exception
    {
        public BudgetTrackerException(string message) : base(message) { }
        public BudgetTrackerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
