using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IncomeService> _logger;

        public IncomeService(IUnitOfWork unitOfWork, ILogger<IncomeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IncomeDTO> CreateIncomeAsync(CreateIncomeCommand createIncomeCommand)
        {
            _logger.LogInformation("Creating a new income with the following details: {Details}", createIncomeCommand);

            // Map command to entity
            Income income = createIncomeCommand.ToEntity();
            income.Medium = createIncomeCommand.Medium;

            // Add to repository via UnitOfWork
            await _unitOfWork.IncomeRepository.AddAsync(income);
            await _unitOfWork.CommitAsync();
            return income.ToDto();

            // Update the active budgets "Income" BudgetItems
            // Retrieve all budgets and select the active one (current date is within the budget's start and end date)
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            var activeBudget = budgets.FirstOrDefault(b => b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);

            if (activeBudget != null)
            {
                // Find the BudgetItem with the category "Income"
                var matchingBudgetItem = activeBudget.BudgetItems.FirstOrDefault(item =>
                    item.Category.Equals("Income", StringComparison.OrdinalIgnoreCase));

                if (matchingBudgetItem != null)
                {
                    // Add the income amount to the BudgetItem's actual value
                    matchingBudgetItem.ActualAmount += income.ActualAmount;
                    // Update the budget container in the repository
                    await _unitOfWork.BudgetRepository.UpdateAsync(activeBudget);
                    await _unitOfWork.CommitAsync();
                }
            }
            _logger.LogInformation("Income created successfully with ID {IncomeId}", income.Id);
            return income.ToDto();
        }
        public async Task<IEnumerable<IncomeDTO>> GetAllIncomesAsync()
        {
            _logger.LogInformation("Retrieving all incomes.");

            var incomes = await _unitOfWork.IncomeRepository.GetAllAsync();
            if (incomes == null || !incomes.Any())
            {
                _logger.LogWarning("No incomes found.");
                return Enumerable.Empty<IncomeDTO>();
            }

            _logger.LogInformation("Retrieved {Count} incomes.", incomes.Count());
            return incomes.Select(i => i.ToDto());
        }
        public async Task<IncomeDTO?> GetIncomeByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving income with ID {IncomeId}", id);

            var income = await _unitOfWork.IncomeRepository.GetByIdAsync(id);
            if (income == null)
            {
                _logger.LogWarning("Income with ID {IncomeId} not found.", id);
                return null;
            }

            _logger.LogInformation("Retrieved income with ID {IncomeId}", id);
            return income.ToDto();
        }
        public async Task<IEnumerable<IncomeDTO>> GetIncomesByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            _logger.LogInformation("Retrieving incomes for budget with ID {BudgetId}", budgetContainerId);
            var incomes = await _unitOfWork.IncomeRepository.FindAsync(i => i.BudgetContainerId == budgetContainerId);
            _logger.LogInformation("Retrieved {Count} incomes for budget with ID {BudgetId}.", incomes.Count(), budgetContainerId);
            return incomes.Select(i => i.ToDto());
        }
        public async Task<bool> UpdateIncomeAsync(UpdateIncomeCommand updateCommand)
        {
            _logger.LogInformation("Updating income with ID {IncomeId}", updateCommand.Id);

            if (updateCommand.Id == Guid.Empty)
            {
                _logger.LogWarning("Invalid ID provided.");
                return false;
            }

            var existingIncome = await _unitOfWork.IncomeRepository.GetByIdAsync(updateCommand.Id);
            if (existingIncome == null)
            {
                return false;
            }

            existingIncome = updateCommand.ToEntity(existingIncome);
            existingIncome.Medium = updateCommand.Medium;

            bool updated = await _unitOfWork.IncomeRepository.UpdateAsync(existingIncome);
            if (updated)
            {
                await _unitOfWork.CommitAsync();
            }

            _logger.LogInformation("Income updated successfully with ID {IncomeId}", existingIncome.Id);
            return updated;

        }
        public async Task<bool> DeleteIncomeAsync(Guid id)
        {
            _logger.LogInformation("Deleting income with ID {IncomeId}", id);
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid ID provided.");
                return false;
            }

            bool deleted = await _unitOfWork.IncomeRepository.DeleteAsync(id);
            if (deleted)
                await _unitOfWork.CommitAsync();
            return deleted;
        }
    }
}

