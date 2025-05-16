using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs
{
    public class PlannedIncomeDTO
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PeriodStart { get; set; }
    }
}
