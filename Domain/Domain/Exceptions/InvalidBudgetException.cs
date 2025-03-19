using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Exceptions
{
    public class InvalidBudgetException : BudgetTrackerException
    {
        public InvalidBudgetException(string message) 
            : base(message)
        {
        }
    }
}
