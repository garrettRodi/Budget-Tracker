using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetReportDTO
    {
        public string Currency {  get; set; }
        public Money BudgetedExpenses { get; set; }
        public Money ActualExpenses { get; set; }
        public Money Difference { get; set; }
    }
}
