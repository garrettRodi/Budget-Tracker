using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Exceptions
{
    public class NotFoundException : BudgetTrackerException
    {
        public NotFoundException(string entityName, Guid id)
            : base($"{entityName} with ID {id} was not found.")
        { }
    }
}
