using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Domain.Entities;
using System;

namespace BudgetTracker.Application.Mappers
{
    public static class IncomeMapper
    {
        // Converts a CreateIncomeCommand to an Income entity
        public static Income ToEntity (this CreateIncomeCommand command)
        {
            return new Income
            {
                Id = Guid.NewGuid(),
                Source = command.Source,
                ActualAmount = command.ActualAmount,
                ReceivedDate = command.ReceivedDate,
                BudgetContainerId = command.BudgetContainerId
            };
        }

        // Updates an existing Income entity with values from an UpdateIncomeCommand
        public static Income ToEntity(this UpdateIncomeCommand command, Income existingIncome)
        {
            existingIncome.Source = command.Source;
            existingIncome.ActualAmount = command.ActualAmount;
            existingIncome.ReceivedDate = command.ReceivedDate;
            return existingIncome;
        }

        // Converts an Income entity to an IncomeDTO
        public static IncomeDTO ToDto(this Income income)
        {
            return new IncomeDTO
            {
                Id = income.Id,
                Source = income.Source,
                ActualAmount = income.ActualAmount,
                ReceivedDate = income.ReceivedDate
            };
        }
    }
}
