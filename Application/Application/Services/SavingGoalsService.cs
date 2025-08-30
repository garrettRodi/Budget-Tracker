using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.Application.Services
{
    public class SavingGoalsService : ISavingGoalsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyService _currencyService;

        public SavingGoalsService(IUnitOfWork unitOfWork, ICurrencyService currencyService)
        {
            _unitOfWork = unitOfWork;
            _currencyService = currencyService;
        }

        public async Task<SavingGoalDTO> CreateSavingGoalAsync(CreateSavingGoalCommand command)
        {

            var goal = command.ToEntity();
            await _unitOfWork.SavingGoalsRepository.AddAsync(goal);
            await _unitOfWork.CommitAsync();

            return goal.ToDto();
        }

        public async Task<SavingGoalDTO?> GetSavingGoalByIdAsync(Guid id)
        {

            var goal = await _unitOfWork.SavingGoalsRepository.GetByIdAsync(id);
            if (goal == null)
            {
                return null;
            }

            return goal != null ? goal.ToDto() : null;
        }

        public async Task<IEnumerable<SavingGoalDTO>> GetSavingGoalsByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            var goals = await _unitOfWork.SavingGoalsRepository.FindAsync(g => g.BudgetContainerId == budgetContainerId);
            return goals.Select(g => g.ToDto());
        }

        public async Task<IEnumerable<SavingGoalDTO>> GetAllSavingGoalsAsync()
        {
            var goals = await _unitOfWork.SavingGoalsRepository.GetAllAsync();
            if (goals == null || !goals.Any())
            {
                return Enumerable.Empty<SavingGoalDTO>();
            }
            return goals.Select(g => g.ToDto());
        }

        public async Task<bool> UpdateSavingGoalAsync(UpdateSavingGoalCommand command) // Method only update editable properties - not CurrentAmount
        {
            var goal = await _unitOfWork.SavingGoalsRepository.GetByIdAsync(command.Id);
            if (goal == null)
                return false;

            goal.GoalName = command.GoalName;
            goal.TargetAmount = command.TargetAmount;
            goal.TargetDate = command.TargetDate;

            var result = await _unitOfWork.SavingGoalsRepository.UpdateAsync(goal);
            if (!result)
            {
                return false;
            }

            await _unitOfWork.CommitAsync();
            return result;
        }

        public async Task RecalculateCurrentAmountAsync(Guid savingGoalId)
        {

            // 1. Fetch the saving goal
            var goal = await _unitOfWork.SavingGoalsRepository.GetByIdAsync(savingGoalId);
            if (goal == null)
            {
                return;
            }

            // 2. Fetch all "Savings" expenses linked to this goal
            var expenses = await _unitOfWork.ExpenseRepository.FindAsync(
                e => e.SavingGoalId == savingGoalId && e.Category == "Savings");

            // 3. Sum all their amounts
            Money total = expenses.Aggregate(new Money(0m, _currencyService.CurrentCurrency), (sum, e) => sum + e.Amount);

            // 4. Update and save
            goal.CurrentAmount = total;
            await _unitOfWork.SavingGoalsRepository.UpdateAsync(goal);
            await _unitOfWork.CommitAsync();

        }

        public async Task<bool> DeleteSavingGoalAsync(Guid id)
        {

            var result = await _unitOfWork.SavingGoalsRepository.DeleteAsync(id);
            if (!result)
            {
                return false;
            }
            await _unitOfWork.CommitAsync();

            return result;
        }
    }
}
