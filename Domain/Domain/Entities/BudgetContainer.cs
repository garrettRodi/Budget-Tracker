using BudgetTracker.Domain.ValueObjects;

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
        public string Currency { get; set; } // Default native currency
        public Money InitialCashBalance { get; set; }
        public Money CurrentCashBalance
        {
            get
            {
                var currency = InitialCashBalance.Currency;
                var zero = new Money(0m, currency);

                var cashIn = Incomes
                    .Where(i => i.Medium == TransactionMedium.Cash)
                    .Select(i => i.ActualAmount)
                    .Aggregate(zero, (sum, m) => sum + m);

                var cashOut = Expenses
                    .Where(e => e.Medium == TransactionMedium.Cash)
                    .Select(e => e.Amount)
                    .Aggregate(zero, (sum, m) => sum + m);

                var transferIn = Transfers
                    .Where(t => t.To == TransactionMedium.Cash)
                    .Select(t => t.Amount)
                    .Aggregate(zero, (sum, m) => sum + m);

                var transferOut = Transfers
                    .Where(t => t.From == TransactionMedium.Cash)
                    .Select(t => t.Amount)
                    .Aggregate(zero, (sum, m) => sum + m);

                return InitialCashBalance
                    + cashIn
                    - cashOut
                    + transferIn
                    - transferOut;
            }
        }
        public Money InitialBankBalance { get; set; }
        public Money CurrentBankBalance
        {
            get
            {
                var currency = InitialBankBalance.Currency;
                var zero = new Money(0m, currency);

                var bankIn = Incomes
                    .Where(i => i.Medium == TransactionMedium.Bank)
                    .Select(i => i.ActualAmount)
                    .Aggregate(zero, (sum, m) => sum + m);

                var bankOut = Expenses
                    .Where(e => e.Medium == TransactionMedium.Bank)
                    .Select(e => e.Amount)
                    .Aggregate(zero, (sum, m) => sum + m);

                var transferIn = Transfers
                    .Where(t => t.To == TransactionMedium.Bank)
                    .Select(t => t.Amount)
                    .Aggregate(zero, (sum, m) => sum + m);

                var transferOut = Transfers
                    .Where(t => t.From == TransactionMedium.Bank)
                    .Select(t => t.Amount)
                    .Aggregate(zero, (sum, m) => sum + m);

                return InitialBankBalance
                    + bankIn
                    - bankOut
                    + transferIn
                    - transferOut;
            }
        }
        public void AddItem(Guid id, string category, Money plannedAmount)
        {
            if (BudgetItems.Any(x => x.Id == id))
                throw new InvalidOperationException($"Item {id} already exists.");

            BudgetItems.Add(new BudgetItem
            {
                Id = id,
                Category = category,
                PlannedAmount = plannedAmount,
                ActualAmount = new Money(0m, plannedAmount.Currency),
                BudgetContainerId = this.Id
            });
        }

        public void RemoveItem(Guid id)
        {
            var item = BudgetItems.FirstOrDefault(x => x.Id == id);
            if (item != null)
                BudgetItems.Remove(item);
        }
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
        public ICollection<PlannedIncome> PlannedIncomes { get; set; } = new List<PlannedIncome>();
        public ICollection<SavingGoals> SavingGoals { get; set; } = new List<SavingGoals>();
        public ICollection<BudgetItem> BudgetItems { get; set; } = new List<BudgetItem>();
        public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
    }
}
