using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.DataAccess;
using BudgetTracker.Infrastructure.Interface;

namespace BudgetTracker.Infrastructure.Logging
{
    public class DatabaseLogger : IBudgetLogger
    {
        private readonly BudgetTrackerDbContext _context;

        public DatabaseLogger(BudgetTrackerDbContext context)
        {
            _context = context;
        }


        public void Log(string message)
        {
            var log = new Log
            {
                Id = Guid.NewGuid(),
                Message = message,
                LoggedAt = DateTime.Now
            };

            _context.Logs.Add(log);
            _context.SaveChangesAsync();
        }
    }
}
