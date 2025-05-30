using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Money Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; }

        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; } = null!;

        public Guid? SavingGoalId { get; set; }
        public SavingGoals? SavingGoal { get; set; }
    }
}
