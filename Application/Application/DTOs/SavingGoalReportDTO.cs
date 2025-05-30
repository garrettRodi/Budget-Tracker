using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class SavingGoalReportDTO
    {
        public Guid Id { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public Money TargetAmount { get; set; }
        public Money CurrentAmount { get; set; }
        public string Currency {  get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
