using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Entities
{
    public class SavingGoals
    {
        public Guid Id { get; set; }
        public string GoalName { get; set; } = string.Empty; // e.g., "Emergency Fund"
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime? TargetDate { get; set; }
    
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; } = null!;
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    }
}
