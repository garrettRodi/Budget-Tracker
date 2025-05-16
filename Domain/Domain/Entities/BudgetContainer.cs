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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
        public ICollection<SavingGoals> SavingGoals { get; set; } = new List<SavingGoals>();
        public ICollection<BudgetItem> BudgetItems { get; set; } = new List<BudgetItem>();
    }
}
