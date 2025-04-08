using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Domain.Entities;
using System;
using System.Linq;

namespace BudgetTracker.Application.Mappers
{
    public static class BudgetMapper
    {
        public static BudgetContainer ToEntity(this CreateBudgetCommand command)
        {
            return new BudgetContainer
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Frequency = command.Frequency,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                AutoRenew = command.AutoRenew,
                Items = command.Items?.Select(item => new BudgetItem
                {
                    Id = Guid.NewGuid(),
                    Category = item.Category,
                    PlannedAmount = item.PlannedAmount,
                    ActualAmount = 0m
                }).ToList()
            };
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
