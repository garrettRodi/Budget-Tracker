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

        /// <summary>
        /// One entry per report‐column: 
        /// - daily dates for weekly/monthly budgets
        /// - month‐start dates for yearly budgets
        /// </summary>
        public List<DateTime> ReportingPeriods { get; set; } = new();

        public List<string> Categories { get; set; } = new();

        public Dictionary<(string Category, DateTime PeriodStart), Money> PlannedByCategoryAndDate
        { get; set; } = new();

        public Dictionary<(string Category, DateTime PeriodStart), Money> ActualByCategoryAndDate
        { get; set; } = new();
    }
}
