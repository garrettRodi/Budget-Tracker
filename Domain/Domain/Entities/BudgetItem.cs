using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Entities
{
    public class BudgetItem
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; }
        public string Category { get; set; } = string.Empty; // e.g., Food, Rent, Utilities, etc.
        public decimal PlannedAmount { get; set; }
        public decimal ActualAmount { get; set; }
    }
}
