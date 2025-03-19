using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.ValueObjects
{
    public class DateRange
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date must be greater or equal to the start date.");
            
            StartDate = startDate;
            EndDate = endDate;
        }

        // Optionally include helper methods (i.e. Duration, Overlap checking, etc.)
        public TimeSpan Duration => EndDate - StartDate;
    }
}
