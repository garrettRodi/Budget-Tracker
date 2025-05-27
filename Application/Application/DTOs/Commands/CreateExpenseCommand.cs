using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreateExpenseCommand
    {
        public Guid BudgetContainerId { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public Guid? SavingGoalId { get; set; }
    }
}
