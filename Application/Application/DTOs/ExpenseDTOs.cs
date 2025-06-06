using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class ExpenseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Money Amount { get; set; }
        public string Currency {  get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public Guid? SavingGoalId { get; set; }
    }
}
