using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreateBudgetItemCommand
    {
        public string Category { get; set; } = string.Empty;
        public Money PlannedAmount { get; set; }
    }
}
