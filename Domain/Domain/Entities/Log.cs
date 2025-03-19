using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Entities
{
    public class Log
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime LoggedAt { get; set; }
    }
}
