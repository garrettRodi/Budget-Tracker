using System;
using System.Collections.Generic;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetMatrixReportDTO
    {
        public string Currency {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Money InitialCashBalance { get; set; }
        public Money InitialBankBalance { get; set; }
        public Money CurrentCashBalance { get; set; }
        public Money CurrentBankBalance { get; set; }
       
        public List<DateTime> ReportingPeriods { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public Dictionary<(string Category, DateTime PeriodStart), Money> PlannedByCategoryAndDate { get; set; } = new();
        public Dictionary<(string Category, DateTime PeriodStart), Money> ActualByCategoryAndDate { get; set; } = new();
    }
}
