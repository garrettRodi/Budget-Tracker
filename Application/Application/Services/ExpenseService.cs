using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Services;
using BudgetTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;
namespace BudgetTracker.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISavingGoalsService _savingGoalsService;
        private readonly ILogger<ExpenseService> _logger; // Use Microsoft.Extensions.Logging for ILogger<T>
        // (Optionally, you can later inject CurrencyConversionService if multi‑currency support is needed.)

        public ExpenseService(IUnitOfWork unitOfWork, ISavingGoalsService savingGoalsService, ILogger<ExpenseService> logger)
        {
            _unitOfWork = unitOfWork;
            _savingGoalsService = savingGoalsService;
            _logger = logger;
        }

        public async Task<ExpenseDTO> CreateExpenseAsync(CreateExpenseCommand createCommand)
        {
           _logger.LogInformation("Creating a new expense with the following details: {Details}", createCommand);

            Expense entity = createCommand.ToEntity();

            // Validate the expense before saving.
            var validator = new ExpenseValidator();
            validator.ValidateExpense(entity);

            // Add the expense to the repository.
            await _unitOfWork.ExpenseRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Updating the BudgetItems actual amount with the new expense: {Expense}", entity);

            // Update the actual amount for the corresponding BudgetItem.
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            var activeBudget = budgets.FirstOrDefault(b => b.StartDate <= DateTime.Now && b.EndDate >= DateTime.Now);

            // If expense category is 'Savings', update the saving goals actual progress.
            if (activeBudget != null)
            {
                // Find a matching BudgetItem by comparing the category.
                var matchingBudgetItem = activeBudget.BudgetItems.FirstOrDefault(item =>
                    item.Category.Equals(entity.Category, StringComparison.OrdinalIgnoreCase));

                if (matchingBudgetItem != null)
                {
                    // Add the expense amount to the BudgetItem's actual value.
                    matchingBudgetItem.ActualAmount += entity.Amount;

                    // Update the budget container in the repository.
                    await _unitOfWork.BudgetRepository.UpdateAsync(activeBudget);
                    await _unitOfWork.CommitAsync();

                    _logger.LogInformation("BudgetItem updated successfully with new actual amount: {ActualAmount}", matchingBudgetItem.ActualAmount);
                }
            }
           
            // Update a Saving Goal if the expense category is 'Savings'.
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

                    _logger.LogInformation("Saving goal updated successfully with new current amount: {CurrentAmount}", activeGoal.CurrentAmount);
                }
            }
            _logger.LogInformation("Expense created successfully with ID: {Id}", entity.Id);

            return entity.ToDto();
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpenseAsync()
        {
            _logger.LogInformation("Retrieving all expenses.");

            var expenses = await _unitOfWork.ExpenseRepository.GetAllAsync();
            if (expenses == null || !expenses.Any())
            {
                _logger.LogWarning("No expenses found.");
                return Enumerable.Empty<ExpenseDTO>();
            }

            _logger.LogInformation("Retrieved {Count} expenses.", expenses.Count());
            return expenses.Select(e => e.ToDto());
        }

        public async Task<ExpenseDTO?> GetExpenseByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving expense with ID: {Id}", id);

            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid ID provided.");
                return null;
            }
            var expense = await _unitOfWork.ExpenseRepository.GetByIdAsync(id);
            if (expense == null)
            {
                _logger.LogWarning("Expense with ID: {Id} not found.", id);
                return null;
            }
            _logger.LogInformation("Expense created successfully with ID: {Id}", id);
            return expense.ToDto();
        }

        public async Task<IEnumerable<ExpenseDTO>> GetExpensesByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            _logger.LogInformation("Retrieving expenses for budget with ID: {BudgetId}", budgetContainerId);

            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(e => e.BudgetContainerId == budgetContainerId);

            _logger.LogInformation("Retrieved {Count} expenses for budget with ID: {BudgetId}", expenses.Count(), budgetContainerId);
            return expenses.Select(e => e.ToDto());
        }

        public async Task<bool> UpdateExpenseAsync(UpdateExpenseCommand updateCommand)
        {
            _logger.LogInformation("Updating expense with ID: {Id}", updateCommand.Id);

            var existingExpense = await _unitOfWork.ExpenseRepository.GetByIdAsync(updateCommand.Id);
            if (existingExpense == null)
                return false;

            // Validate updated expense.
            var validator = new ExpenseValidator();
            validator.ValidateExpense(existingExpense);

            updateCommand.ToEntity(existingExpense);

            _logger.LogInformation("Expense updated successfully with ID: {Id}", existingExpense.Id);

            return await _unitOfWork.ExpenseRepository.UpdateAsync(existingExpense);
        }

        public async Task<bool> DeleteExpenseAsync(Guid id)
        {
            _logger.LogInformation("Deleting expense with ID: {Id}", id);
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid ID provided.");
                return false;
            }

            bool result = await _unitOfWork.ExpenseRepository.DeleteAsync(id);
            if (result)
            {
                // Persist changes to the database.
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Expense deleted successfully with ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete expense with ID: {Id}", id);
            }
            return result;
        }

        public async Task<decimal> GetTotalExpensesAsync()
        {
            _logger.LogInformation("Calculating total expenses.");

            var expenses = await _unitOfWork.ExpenseRepository.GetAllAsync();
            if (expenses == null || !expenses.Any())
            {
                _logger.LogWarning("No expenses found.");
                return 0;
            }

            _logger.LogInformation("Total expenses calculated: {Total}", expenses.Sum(e => e.Amount));
            return expenses.Sum(e => e.Amount);
        }
    }
}
