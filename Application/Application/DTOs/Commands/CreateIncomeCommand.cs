using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class CreateIncomeCommand
    {
        public Guid BudgetContainerId { get; set; }

        public string Source { get; set; } = string.Empty;
        public decimal ActualAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
