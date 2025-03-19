using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs
{
    public class ExpenseReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalExpenses { get; set; }
        // Total amount per expense category.
        public Dictionary<string, decimal> CategoryTotals { get; set; } = new Dictionary<string, decimal>();
        // Percentage thwt each category contributes to the total expenses.
        public Dictionary<string, decimal> CategoryPercentages { get; set; } = new Dictionary<string, decimal>();
    }
}
