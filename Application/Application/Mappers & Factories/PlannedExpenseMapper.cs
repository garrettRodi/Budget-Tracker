using System;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Mappers
{
    public static class PlannedExpenseMapper
    {
        public static PlannedExpense ToEntity(this CreatePlannedExpenseCommand cmd)
            => new()
            {
                Id = Guid.NewGuid(),
                BudgetContainerId = cmd.BudgetContainerId,
                Category = cmd.Category,
                Amount = cmd.Amount,
                Period = cmd.Period
            };

        public static PlannedExpense ToEntity(this UpdatePlannedExpenseCommand cmd, PlannedExpense existing)
        {
            existing.Category = cmd.Category;
            existing.Amount = cmd.Amount;
            existing.Period = cmd.Period;
            return existing;
        }

        public static PlannedExpenseDTO ToDto(this PlannedExpense pe)
            => new()
            {
                Id = pe.Id,
                Category = pe.Category,
                Amount = pe.Amount,
                Period = pe.Period
            };
    }
}
