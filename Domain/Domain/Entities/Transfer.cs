using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Domain.Entities
{
    public enum TransactionMedium
    {
        Cash,
        Bank
    }
    public class Transfer
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public BudgetContainer BudgetContainer { get; set; } = null!;
        public TransactionMedium From { get; set; }
        public TransactionMedium To { get; set; }
        public Money Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
