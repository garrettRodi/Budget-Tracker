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

namespace BudgetTracker.Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IncomeDTO> CreateIncomeAsync(CreateIncomeCommand createIncomeCommand)
        {
            // Map command to entity
            Income income = createIncomeCommand.ToEntity();

            // Add to repository via UnitOfWork
            await _unitOfWork.IncomeRepository.AddAsync(income);
            await _unitOfWork.CommitAsync();

            // Update the active budgets "Income" BudgetItems
            // Retrieve all budgets and select the active one (current date is within the budget's start and end date)
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            var activeBudget = budgets.FirstOrDefault(b => b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);

            if (activeBudget != null)
            {
                // Find the BudgetItem with the category "Income"
                var matchingBudgetItem = activeBudget.Items.FirstOrDefault(item =>
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
            // Convert entity to DTO and return
            return income.ToDto();
        }

        public async Task<IEnumerable<IncomeDTO>> GetAllIncomesAsync()
        {
            var incomes = await _unitOfWork.IncomeRepository.GetAllAsync();
            return incomes.Select(i => i.ToDto());
        }

        public async Task<IncomeDTO?> GetIncomeByIdAsync(Guid id)
        {
            var income = await _unitOfWork.IncomeRepository.GetByIdAsync(id);
            return income != null ? income.ToDto() : null;
        }

        public async Task<bool> UpdateIncomeAsync(UpdateIncomeCommand updateCommand)
        {
            var existingIncome = await _unitOfWork.IncomeRepository.GetByIdAsync(updateCommand.Id);
            if (existingIncome == null)
                return false;

            existingIncome = updateCommand.ToEntity(existingIncome);
            bool updated = await _unitOfWork.IncomeRepository.UpdateAsync(existingIncome);
            if (updated)
                await _unitOfWork.CommitAsync();
            return updated;
        }

        public async Task<bool> DeleteIncomeAsync(Guid id)
        {
            bool deleted = await _unitOfWork.IncomeRepository.DeleteAsync(id);
            if (deleted)
                await _unitOfWork.CommitAsync();
            return deleted;
        }
    }
}

