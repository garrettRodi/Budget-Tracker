using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class UpdateBudgetCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public BudgetFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public decimal InitialCashBalance { get; set; }
        public decimal InitialBankBalance { get; set; }
        public string Currency { get; set; }

        public List<UpdateBudgetItemCommand> Items { get; set; } = new();
    }
}
