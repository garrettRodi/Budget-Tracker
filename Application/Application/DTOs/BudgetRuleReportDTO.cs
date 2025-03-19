using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetRuleReportDTO
    {
        // Example: 50/20/30
        public string Rule { get; set; } = string.Empty;
        public decimal NecessitiesPlanned { get; set; }
        public decimal NecessitiesActual { get; set; }
        public decimal SavingsPlanned { get; set; }
        public decimal SavingsActual { get; set; }
        public decimal DiscretionaryPlanned { get; set; }
        public decimal DiscretionaryActual { get; set; }
        // Variance in percentages between planned and actual spending.
        public decimal NecessitiesPercentageVariance { get; set; }
        public decimal SavingsPercentageVariance { get; set; }
        public decimal DiscretionaryPercentageVariance { get; set; }
    }
}
