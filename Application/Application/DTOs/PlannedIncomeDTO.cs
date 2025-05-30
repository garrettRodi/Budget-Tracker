using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class PlannedIncomeDTO
    {
        public Guid Id { get; set; }
        public Guid BudgetContainerId { get; set; }
        public Money Amount { get; set; }
        public string Currency {  get; set; }
        public DateTime PeriodStart { get; set; }
    }
}
