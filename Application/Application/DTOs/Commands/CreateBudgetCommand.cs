using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreateBudgetCommand
    {
        public string Name { get; set; } = string.Empty;
        public BudgetFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }

        // List of budget items with their planned amounts.
        public List<CreateBudgetItemCommand> Items { get; set; } = new List<CreateBudgetItemCommand>();
    }
}
