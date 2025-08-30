using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public BudgetFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; }
        public string Currency { get; set; }
        public decimal InitialCashBalance { get; set; }
        public decimal InitialBankBalance { get; set; }
        public decimal CurrentCashBalance { get; set; }
        public decimal CurrentBankBalance { get; set; }

        public List<BudgetItemDTO> Items { get; set; } = new();

    }

    public class BudgetItemDTO
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
    } 
}
