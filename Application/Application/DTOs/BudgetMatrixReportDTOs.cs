using System;
using System.Collections.Generic;

namespace BudgetTracker.Application.DTOs
{
    public class BudgetMatrixReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// One entry per report‐column: 
        /// - daily dates for weekly/monthly budgets
        /// - month‐start dates for yearly budgets
        /// </summary>
        public List<DateTime> ReportingPeriods { get; set; } = new();

        public List<string> Categories { get; set; } = new();

        public Dictionary<(string Category, DateTime PeriodStart), decimal> PlannedByCategoryAndDate
        { get; set; } = new();

        public Dictionary<(string Category, DateTime PeriodStart), decimal> ActualByCategoryAndDate
        { get; set; } = new();
    }
}
