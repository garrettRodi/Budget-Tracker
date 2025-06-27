using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreatePlannedIncomeCommand
    {
        public Guid BudgetContainerId { get; set; }
        public string Source { get; set; } = null!;
        public Money Amount { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
    }
}
