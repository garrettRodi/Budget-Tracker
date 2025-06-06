using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Interfaces
{
    public interface ICurrencyService
    {
        string CurrentCurrency { get; set; }
    }
}
