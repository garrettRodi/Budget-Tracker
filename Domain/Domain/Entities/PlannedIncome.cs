using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities
{
    public class PlannedIncome
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; } = null!;
        public string Source { get; set; } = null!;
        public Money Amount { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
    }
}
