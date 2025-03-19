using Microsoft.EntityFrameworkCore;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.EntityConfigurations;

namespace BudgetTracker.Infrastructure.DataAccess
{
    public class BudgetTrackerDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Income> Incomes { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Log> Logs { get; set; } = null!;
        public DbSet<BudgetContainer> BudgetContainers { get; set; } = null!;
        public DbSet<BudgetItem> BudgetItems { get; set; } = null!;
        public DbSet<SavingGoals> SavingGoals { get; set; } = null!;
        public DbSet<CategoryMapping> CategoryMappings { get; set; } = null!;

        public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply any entity configurations (like ExpenseConfiguration)
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.Entity<CategoryMapping>().ToTable("CategoryMappings");

            // Configure the one-to-many relationship between BudgetContainer and BudgetItem.
            modelBuilder.Entity<BudgetContainer>()
                .HasMany(b => b.Items)
                .WithOne(i => i.BudgetContainer)
                .HasForeignKey(i => i.BudgetContainerId)
                .OnDelete(DeleteBehavior.Cascade); // Cascading delete if a container is removed.

            // Existing seed data for Categories.
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Food" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Rent" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Entertainment" }
            );

            // Seed default mappings (you can adjust these as needed)
            modelBuilder.Entity<CategoryMapping>().HasData(
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000001"),
                    CategoryName = "Food",
                    GroupName = "Necessities"
                },
    new CategoryMapping
    {
        Id = new Guid("00000000-0000-0000-0000-000000000002"),
        CategoryName = "Rent",
        GroupName = "Necessities"
    },
    new CategoryMapping
    {
        Id = new Guid("00000000-0000-0000-0000-000000000003"),
        CategoryName = "Entertainment",
        GroupName = "Discretionary"
    },
    new CategoryMapping
    {
        Id = new Guid("00000000-0000-0000-0000-000000000004"),
        CategoryName = "Investments",
        GroupName = "Savings"
    }
            );
        }
    }
}
