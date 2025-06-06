using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.DTOs
{
    public class IncomeReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Money TotalIncome { get; set; }
        public string Currency {  get; set; }
    }
}
