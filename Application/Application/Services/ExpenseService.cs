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

            await _unitOfWork.ExpenseRepository.AddAsync(entity);

            // If expense category is 'Savings', update the saving goals actual progress.
            if (entity.Category.Equals("Savings", StringComparison.OrdinalIgnoreCase))
            {
                // Fetch all saving goals.
                var savingGoals = await _savingGoalsService.GetAllSavingGoalsAsync();
                // Select the first active saving goal (The one that hasn't met its target yet).
                var activeGoal = savingGoals.FirstOrDefault(sg => sg.CurrentAmount < sg.TargetAmount);
                if (activeGoal != null)
                {
                    // Update the active saving goal's current amount by adding the expense amount.
                    activeGoal.CurrentAmount += entity.Amount;

                    // Create update command for saving goal
                    var updateCommand = new UpdateSavingGoalCommand
                    {
                        Id = activeGoal.Id,
                        GoalName = activeGoal.GoalName,
                        TargetAmount = activeGoal.TargetAmount,
                        CurrentAmount = activeGoal.CurrentAmount,
                        TargetDate = activeGoal.TargetDate
                    };
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
