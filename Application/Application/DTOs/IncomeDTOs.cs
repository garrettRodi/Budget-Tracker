using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class IncomeDTO
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Currency { get; set; }
        public Money ActualAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public TransactionMedium Medium { get; set; }
    }
}
