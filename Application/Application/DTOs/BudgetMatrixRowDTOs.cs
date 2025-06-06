using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTrackers.Application.DTOs
{

    public class BudgetMatrixRowDTO
    {
        public string ItemName { get; set; } = default!;
        public string Currency {  get; set; }
        public Dictionary<DateTime, (Money Planned, Money Actual)> Values { get; set; } = [];
    }
}
