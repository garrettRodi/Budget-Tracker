using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Interfaces;

namespace BudgetTracker.Application.Services
{
    public class SavingGoalsService : ISavingGoalsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SavingGoalsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            return goal != null ? goal.ToDto() : null;
        }

        public async Task<IEnumerable<SavingGoalDTO>> GetAllSavingGoalsAsync()
        {
            var goals = await _unitOfWork.SavingGoalsRepository.GetAllAsync();
            return goals.Select(g => g.ToDto());
        }

        public async Task<bool> UpdateSavingGoalAsync(UpdateSavingGoalCommand command)
        {
            var goal = await _unitOfWork.SavingGoalsRepository.GetByIdAsync(command.Id);
            if (goal == null)
                return false;

            // Update the goal with the new data.
            goal.GoalName = command.GoalName;
            goal.TargetAmount = command.TargetAmount;
            goal.CurrentAmount = command.CurrentAmount;
            goal.TargetDate = command.TargetDate;

            var result = await _unitOfWork.SavingGoalsRepository.UpdateAsync(goal);
            await _unitOfWork.CommitAsync();
            return result;
        }

        public async Task<bool> DeleteSavingGoalAsync(Guid id)
        {
            var result = await _unitOfWork.SavingGoalsRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return result;
        }
    }
}
