using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;

namespace BudgetTracker.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        public string CurrentCurrency { get; set; } = "USD";
    }
}
