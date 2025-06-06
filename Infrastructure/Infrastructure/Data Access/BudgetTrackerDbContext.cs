using Microsoft.EntityFrameworkCore;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;   // ←– Needed to reference Money
using BudgetTracker.Infrastructure.EntityConfigurations;
using BudgetTracker.Domain.ValueObjects;

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
        public DbSet<PlannedIncome> PlannedIncomes { get; set; } = null!;
        public DbSet<PlannedExpense> PlannedExpenses { get; set; } = null!;

        public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity‐specific configurations (e.g. ExpenseConfiguration)
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.Entity<CategoryMapping>().ToTable("CategoryMappings");

            //
            // ───── RELATIONSHIPS & CASCADING DELETES ───────────────────
            //

            // BudgetContainer → BudgetItems (1 : N) with cascade delete
            modelBuilder.Entity<BudgetContainer>()
                .HasMany(b => b.BudgetItems)
                .WithOne(i => i.BudgetContainer)
                .HasForeignKey(i => i.BudgetContainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // BudgetContainer → Expenses (1 : N) with cascade delete
            modelBuilder.Entity<BudgetContainer>()
                .HasMany(b => b.Expenses)
                .WithOne(e => e.BudgetContainer)
                .HasForeignKey(e => e.BudgetContainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // BudgetContainer → Incomes (1 : N) with cascade delete
            modelBuilder.Entity<BudgetContainer>()
                .HasMany(b => b.Incomes)
                .WithOne(i => i.BudgetContainer)
                .HasForeignKey(i => i.BudgetContainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlannedIncome → BudgetContainer (1 : N) with cascade delete
            modelBuilder.Entity<PlannedIncome>()
                .ToTable("PlannedIncomes")
                .HasKey(pi => pi.Id);
            modelBuilder.Entity<PlannedIncome>()
                .HasOne(pi => pi.BudgetContainer)
                .WithMany(b => b.PlannedIncomes)
                .HasForeignKey(pi => pi.BudgetContainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // BudgetContainer → SavingGoals (1 : N) with cascade delete
            modelBuilder.Entity<BudgetContainer>()
                .HasMany(b => b.SavingGoals)
                .WithOne(s => s.BudgetContainer)
                .HasForeignKey(s => s.BudgetContainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Expense → SavingGoals (N : 1) with cascade delete
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.SavingGoal)
                .WithMany()
                .HasForeignKey(e => e.SavingGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            //
            // ──────── OWNED TYPE MAPPINGS FOR Money ───────────────────
            //
            // We explicitly map each Money property into two columns:
            //  - [PropertyName]_Amount   (decimal(18,2))
            //  - [PropertyName]_Currency (nvarchar(3), default "USD")
            // This ensures EF creates those columns and persists both parts.
            //

            // ===== Expense.Amount (Money) =====
            modelBuilder.Entity<Expense>()
                .OwnsOne(e => e.Amount, moneyBuilder =>
                {
                    // Map the decimal Amount → column "Amount_Amount"
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("Amount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    // Map the Currency → column "Amount_Currency", default "USD"
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("Amount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== Income.ActualAmount (Money) =====
            modelBuilder.Entity<Income>()
                .OwnsOne(i => i.ActualAmount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("ActualAmount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("ActualAmount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== PlannedExpense.Amount (Money) =====
            modelBuilder.Entity<PlannedExpense>()
                .OwnsOne(pe => pe.Amount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("Amount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("Amount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== PlannedIncome.Amount (Money) =====
            modelBuilder.Entity<PlannedIncome>()
                .OwnsOne(pi => pi.Amount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("Amount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("Amount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== BudgetItem.PlannedAmount (Money) =====
            modelBuilder.Entity<BudgetItem>()
                .OwnsOne(bi => bi.PlannedAmount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("PlannedAmount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("PlannedAmount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== BudgetItem.ActualAmount (Money) =====
            modelBuilder.Entity<BudgetItem>()
                .OwnsOne(bi => bi.ActualAmount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("ActualAmount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("ActualAmount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== SavingGoals.TargetAmount (Money) =====
            modelBuilder.Entity<SavingGoals>()
                .OwnsOne(sg => sg.TargetAmount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("TargetAmount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("TargetAmount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            // ===== SavingGoals.CurrentAmount (Money) =====
            modelBuilder.Entity<SavingGoals>()
                .OwnsOne(sg => sg.CurrentAmount, moneyBuilder =>
                {
                    moneyBuilder.Property(m => m.Amount)
                        .HasColumnName("CurrentAmount_Amount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    moneyBuilder.Property(m => m.Currency)
                        .HasColumnName("CurrentAmount_Currency")
                        .HasMaxLength(3)
                        .IsRequired()
                        .HasDefaultValue("USD");
                });

            //
            // ─────── EXISTING SEED DATA FOR CATEGORY & CATEGORY MAPPING ──
            //
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Food" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Rent" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Entertainment" }
            );

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
                },
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000005"),
                    CategoryName = "Utilities",
                    GroupName = "Necessities"
                },
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000006"),
                    CategoryName = "Transportation",
                    GroupName = "Necessities"
                },
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000007"),
                    CategoryName = "Healthcare",
                    GroupName = "Necessities"
                },
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000008"),
                    CategoryName = "Clothing",
                    GroupName = "Discretionary"
                },
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000009"),
                    CategoryName = "Education",
                    GroupName = "Discretionary"
                },
                new CategoryMapping
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000010"),
                    CategoryName = "Miscellaneous",
                    GroupName = "Discretionary"
                }
            );

            // CHANGE: Map Money as an owned type for Income
            modelBuilder.Entity<Income>().OwnsOne(i => i.Amount, owned =>
            {
                owned.Property(m => m.Amount).HasColumnName("Amount");      // CHANGE: Maps Money.Amount
                owned.Property(m => m.Currency).HasColumnName("Currency");  // CHANGE: Maps Money.Currency
            });

            // CHANGE: Map Money as an owned type for Expense
            modelBuilder.Entity<Expense>().OwnsOne(e => e.Amount, owned =>
            {
                owned.Property(m => m.Amount).HasColumnName("Amount");      // CHANGE: Maps Money.Amount
                owned.Property(m => m.Currency).HasColumnName("Currency");  // CHANGE: Maps Money.Currency
            });
        }
    }
    }
}
