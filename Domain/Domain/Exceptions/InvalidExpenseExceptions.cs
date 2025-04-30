using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Exceptions
{
    public class InvalidExpenseException : BudgetTrackerException
    {
        public InvalidExpenseException(decimal amount) 
            : base ($"Invalid amount for expense: {amount}")
        {
        }
    }
    // Can add more domain-specific exceptions here if needed
}
