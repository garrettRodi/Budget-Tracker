using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Entities
{
    public class CategoryMapping
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        // The group to which the category belongs (e.g., "Necessities", "Savings", "Discretionary")
        public string GroupName { get; set; } = string.Empty;
    }
}

