using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTrackers.Application.DTOs
{

    public class BudgetMatrixRowDTO
    {
        public string ItemName { get; set; } = default!;
        public Dictionary<DateTime, (decimal Planned, decimal Actual)> Values { get; set; } = [];
    }
}
