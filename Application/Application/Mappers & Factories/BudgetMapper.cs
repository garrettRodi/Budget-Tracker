using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using System;
using System.Linq;

namespace BudgetTracker.Application.Mappers
{
    public static class BudgetMapper
    {
        public static BudgetContainer ToEntity(this CreateBudgetCommand command)
        {
            // 1) Create the parent and generate its ID
            var budget = new BudgetContainer
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Frequency = command.Frequency,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                AutoRenew = command.AutoRenew
            };

            // 2) Now map each item, using the parent’s ID
            if (command.Items != null)
            {
                budget.BudgetItems = command.Items.Select(item => new BudgetItem
                {
                    Id = Guid.NewGuid(),
                    Category = item.Category,
                    PlannedAmount = item.PlannedAmount,
                    ActualAmount = new Money(0m,item.PlannedAmount.Currency),
                    BudgetContainerId = budget.Id   // ← use budget.Id here
                }).ToList();
            }

            return budget;
        }

        public static BudgetDTO ToDto(this BudgetContainer budget)
        {
            return new BudgetDTO
            {
                Id = budget.Id,
                Name = budget.Name,
                Frequency = budget.Frequency,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                AutoRenew = budget.AutoRenew
            };
        }
    }
}
