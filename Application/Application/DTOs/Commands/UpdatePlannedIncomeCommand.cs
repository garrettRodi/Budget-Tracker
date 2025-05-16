using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs.Commands
{
    public class UpdatePlannedIncomeCommand
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PeriodStart { get; set; }
    }
}
