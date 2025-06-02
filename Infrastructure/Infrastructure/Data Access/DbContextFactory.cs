// File: 04 - Infrastructure/DataAccess/BudgetTrackerDbContextFactory.cs
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BudgetTracker.Infrastructure.DataAccess
{
    public class BudgetTrackerDbContextFactory
        : IDesignTimeDbContextFactory<BudgetTrackerDbContext>
    {
        public BudgetTrackerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BudgetTrackerDbContext>();
            Directory.CreateDirectory("Database");
            var dbFile = Path.Combine("Database", "BudgetTracker.db");

            builder.UseSqlite(
                $"Data Source={dbFile}",
                b => b.MigrationsAssembly("04 - Infrastructure"));

            return new BudgetTrackerDbContext(builder.Options);
        }
    }
}
