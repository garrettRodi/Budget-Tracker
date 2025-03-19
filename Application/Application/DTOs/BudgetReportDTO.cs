using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetReportDTO
    {
        public decimal BudgetedExpenses { get; set; }
        public decimal ActualExpenses { get; set; }
        public decimal Difference { get; set; }
    }
}
