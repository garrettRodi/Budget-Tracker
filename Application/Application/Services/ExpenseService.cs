using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Services;
using BudgetTracker.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace BudgetTracker.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISavingGoalsService _savingGoalsService;
        // (Optionally, you can later inject CurrencyConversionService if multi‑currency support is needed.)

        public ExpenseService(IUnitOfWork unitOfWork, ISavingGoalsService savingGoalsService)
        {
            _unitOfWork = unitOfWork;
            _savingGoalsService = savingGoalsService;
        }

        public async Task<ExpenseDTO> CreateExpenseAsync(CreateExpenseCommand createCommand)
        {
            Expense entity = createCommand.ToEntity();

            // Validate the expense before saving.
            var validator = new ExpenseValidator();
            validator.ValidateExpense(entity);

            // Add the expense to the repository.
            await _unitOfWork.ExpenseRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            // Update the actual amount for the corresponding BudgetItem.
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            var activeBudget = budgets.FirstOrDefault(b => b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);

            // If expense category is 'Savings', update the saving goals actual progress.
            if (activeBudget != null)
            {
                // Find a matching BudgetItem by comparing the category.
                var matchingBudgetItem = activeBudget.Items.FirstOrDefault(item =>
                    item.Category.Equals(entity.Category, StringComparison.OrdinalIgnoreCase));

                if (matchingBudgetItem != null)
                {
                    // Add the expense amount to the BudgetItem's actual value.
                    matchingBudgetItem.ActualAmount += entity.Amount;

                    // Update the budget container in the repository.
                    await _unitOfWork.BudgetRepository.UpdateAsync(activeBudget);
                    await _unitOfWork.CommitAsync();
                }
            }
           
            // Update a Saving Goal ig the expense category is 'Savings'.
            if (entity.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                // Get all saving goals using SavingGoalsService.
                var savingGoals = await _savingGoalsService.GetAllSavingGoalsAsync();

                // Choose the first active saving goal (the one that hasn't met its target).
                var activeGoal = savingGoals.FirstOrDefault(g => g.CurrentAmount < g.TargetAmount);
                if (activeGoal != null)
                {
                    // Add the expense amount to the saving goal's current amount.
                    activeGoal.CurrentAmount += entity.Amount;
                    // Prepare an update command.
                    var updateCommand = new BudgetTracker.Application.DTOs.Commands.UpdateSavingGoalCommand
                    {
                        Id = activeGoal.Id,
                        GoalName = activeGoal.GoalName,
                        TargetAmount = activeGoal.TargetAmount,
                        CurrentAmount = activeGoal.CurrentAmount,
                        TargetDate = activeGoal.TargetDate
                    };

                    // Update the saving goal in the repository.
                    await _savingGoalsService.UpdateSavingGoalAsync(updateCommand);
                }
            }

            return entity.ToDto();
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpenseAsync()
        {
            var expenses = await _unitOfWork.ExpenseRepository.GetAllAsync();
            return expenses.Select(e => e.ToDto());
        }

        public async Task<ExpenseDTO?> GetExpenseByIdAsync(Guid id)
        {
            var expense = await _unitOfWork.ExpenseRepository.GetByIdAsync(id);
            if (expense == null)
                return null;

            return expense.ToDto();
        }

        public async Task<bool> UpdateExpenseAsync(UpdateExpenseCommand updateCommand)
        {
            var existingExpense = await _unitOfWork.ExpenseRepository.GetByIdAsync(updateCommand.Id);
            if (existingExpense == null)
                return false;

            // Validate updated expense.
            var validator = new ExpenseValidator();
            validator.ValidateExpense(existingExpense);

            updateCommand.ToEntity(existingExpense);

            return await _unitOfWork.ExpenseRepository.UpdateAsync(existingExpense);
        }

        public async Task<bool> DeleteExpenseAsync(Guid id)
        {
            return await _unitOfWork.ExpenseRepository.DeleteAsync(id);
        }

        public async Task<decimal> GetTotalExpensesAsync()
        {
            var expenses = await _unitOfWork.ExpenseRepository.GetAllAsync();
            return expenses.Sum(e => e.Amount);
        }
    }
}
