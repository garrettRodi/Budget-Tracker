using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities
{
    public class Income
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public Money ActualAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
    
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; } = null!;
        public TransactionMedium Medium { get; set; }
    }
}
