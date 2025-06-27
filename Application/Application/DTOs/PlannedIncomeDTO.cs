using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class PlannedIncomeDTO
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public string Source { get; set; } = null!;
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
    }
}
