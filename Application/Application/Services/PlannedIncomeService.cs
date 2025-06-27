using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Application.Services
{
    public class PlannedIncomeService : IPlannedIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlannedIncomeService> _logger;

        public PlannedIncomeService(
            IUnitOfWork unitOfWork,
            ILogger<PlannedIncomeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PlannedIncomeDTO> CreatePlannedIncomeAsync(CreatePlannedIncomeCommand command)
        {
            var entity = new PlannedIncome
            {
                Source = command.Source,
                Id = Guid.NewGuid(),
                BudgetContainerId = command.BudgetContainerId,
                Amount = command.Amount,
                PeriodStart = command.PeriodStart
            };
            await _unitOfWork.PlannedIncomeRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Created PlannedIncome {Id}", entity.Id);

            return new PlannedIncomeDTO
            {
                Source = command.Source,
                Id = entity.Id,
                BudgetContainerId = entity.BudgetContainerId,
                Amount = entity.Amount,
                PeriodStart = entity.PeriodStart
            };
        }

        public async Task<IEnumerable<PlannedIncomeDTO>> GetPlannedIncomesByBudgetAsync(Guid budgetContainerId)
        {
            var list = await _unitOfWork.PlannedIncomeRepository.FindByBudgetAsync(budgetContainerId);
            
            return list.Select(pi => new PlannedIncomeDTO
            {
                Source = pi.Source,
                Id = pi.Id,
                BudgetContainerId = pi.BudgetContainerId,
                Amount = pi.Amount,
                PeriodStart = pi.PeriodStart
            });
        }

        public async Task<bool> UpdatePlannedIncomeAsync(UpdatePlannedIncomeCommand command)
        {
            var existing = await _unitOfWork.PlannedIncomeRepository.GetByIdAsync(command.Id);
            if (existing == null) return false;
            
            existing.Source = command.Source;
            existing.Amount = command.Amount;
            existing.PeriodStart = command.PeriodStart;
            var success = await _unitOfWork.PlannedIncomeRepository.UpdateAsync(existing);
            if(success)
            {
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Updated PlannedIncome {Id}", existing.Id);
            }
            return success;
        }

        public async Task<bool> DeletePlannedIncomeAsync(Guid id)
        {
            var deleted = await _unitOfWork.PlannedIncomeRepository.DeleteAsync(id);
            if (deleted)
            {
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Deleted PlannedIncome {Id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete PlannedIncome {Id}", id);
            }
            return deleted;
        }
    }
}
