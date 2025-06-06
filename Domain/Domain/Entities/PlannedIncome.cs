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

        // FK to the budget container
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; } = null!;

        // Amount of planned income
        public Money Amount { get; set; }

        // Period that applies to the planned income
        public DateTime PeriodStart { get; set; }
    }
}
