using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Entities
{
    public enum BudgetFrequency
    {
        Weekly,
        Monthly,
        Yearly
    }

    public class BudgetContainer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public BudgetFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; } // Must not be in the past
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }

        public ICollection<Expense> Expenses { get; set; } = [];
        public ICollection<Income> Incomes { get; set; } = [];
        public ICollection<SavingGoals> SavingGoals { get; set; } = [];
        public ICollection<BudgetItem> BudgetItems { get; set; } = [];
    }
}
