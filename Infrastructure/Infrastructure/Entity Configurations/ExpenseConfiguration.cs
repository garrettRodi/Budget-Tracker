using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.EntityConfigurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            // Example Configurations
            builder.ToTable("Expenses");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            // builder.Property(e => e.Amount).IsRequired();

            // If you want to ensure date can't be in the future, you'd normally do that in domain logic
            // or as a constraint, but this is just an example of how you'd refine your schema.
        }
    }
}
