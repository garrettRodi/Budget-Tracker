using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Helpers;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Mappers
{
    public static class ExpenseMapper
    {
        public static Expense ToEntity(this CreateExpenseCommand dto)
        {
            return new Expense
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Amount = dto.Amount,
                ExpenseDate = dto.Date,
                Category = dto.Category,
                BudgetContainerId = dto.BudgetContainerId,
                SavingGoalId = dto.SavingGoalId
            };
        }

        public static Expense ToEntity(this UpdateExpenseCommand dto, Expense existingExpense)
        {
            existingExpense.Name = dto.Name;
            existingExpense.Amount = dto.Amount;
            existingExpense.ExpenseDate = dto.Date;
            existingExpense.Category = dto.Category;
            existingExpense.SavingGoalId = dto.SavingGoalId;
            return existingExpense;
        }

        public static ExpenseDTO ToDto(this Expense expense)
        {
            return new ExpenseDTO
            {
                Id = expense.Id,
                Name = expense.Name,
                Amount = expense.Amount,
                Date = expense.ExpenseDate,
                Category = expense.Category,
                SavingGoalId = expense.SavingGoalId
            };
        }
    }
}
