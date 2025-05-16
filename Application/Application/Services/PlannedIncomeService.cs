using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Application.Services
{
    public class PlannedIncomeService : IPlannedIncomeService
    {
        private readonly IPlannedIncomeRepository _plannedRepo;
        private readonly ILogger<PlannedIncomeService> _logger;

        public PlannedIncomeService(
            IPlannedIncomeRepository plannedRepo,
            ILogger<PlannedIncomeService> logger)
        {
            _plannedRepo = plannedRepo;
            _logger = logger;
        }

        public async Task<PlannedIncomeDTO> CreatePlannedIncomeAsync(CreatePlannedIncomeCommand command)
        {
            var entity = new PlannedIncome
            {
                Id = Guid.NewGuid(),
                BudgetContainerId = command.BudgetContainerId,
                Amount = command.Amount,
                PeriodStart = command.PeriodStart
            };
            await _plannedRepo.AddAsync(entity);
            _logger.LogInformation("Created PlannedIncome {Id}", entity.Id);

            return new PlannedIncomeDTO
            {
                Id = entity.Id,
                BudgetContainerId = entity.BudgetContainerId,
                Amount = entity.Amount,
                PeriodStart = entity.PeriodStart
            };
        }

        public async Task<IEnumerable<PlannedIncomeDTO>> GetPlannedIncomesByBudgetAsync(Guid budgetContainerId)
        {
            var list = await _plannedRepo.FindByBudgetAsync(budgetContainerId);
            return list.Select(pi => new PlannedIncomeDTO
            {
                Id = pi.Id,
                BudgetContainerId = pi.BudgetContainerId,
                Amount = pi.Amount,
                PeriodStart = pi.PeriodStart
            });
        }

        public async Task<bool> UpdatePlannedIncomeAsync(UpdatePlannedIncomeCommand command)
        {
            var existing = await _plannedRepo.GetByIdAsync(command.Id);
            if (existing == null) return false;

            existing.Amount = command.Amount;
            existing.PeriodStart = command.PeriodStart;
            return await _plannedRepo.UpdateAsync(existing);
        }

        public async Task<bool> DeletePlannedIncomeAsync(Guid id)
            => await _plannedRepo.DeleteAsync(id);
    }
}
