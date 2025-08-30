using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class UpdateBudgetItemCommand
{
        public Guid? Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
    }
}
