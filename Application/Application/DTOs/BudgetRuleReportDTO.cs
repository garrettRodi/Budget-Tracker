using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetRuleReportDTO
    {
        // Example: 50/20/30
        public string Rule { get; set; } = string.Empty;
        public string Currency {  get; set; }
        public Money InitialCashBalance { get; set; }
        public Money InitialBankBalance { get; set; }
        public Money CurrentCashBalance { get; set; }
        public Money CurrentBankBalance { get; set; }
        public Money NecessitiesPlanned { get; set; }
        public Money NecessitiesActual { get; set; }
        public Money SavingsPlanned { get; set; }
        public Money SavingsActual { get; set; }
        public Money DiscretionaryPlanned { get; set; }
        public Money DiscretionaryActual { get; set; }
        // Variance in percentages between planned and actual spending.
        public decimal NecessitiesPercentageVariance { get; set; }
        public decimal SavingsPercentageVariance { get; set; }
        public decimal DiscretionaryPercentageVariance { get; set; }
    }
}
