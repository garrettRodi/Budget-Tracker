using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Application.Services
{
    public class SavingGoalsService : ISavingGoalsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SavingGoalsService> _logger;

        public SavingGoalsService(IUnitOfWork unitOfWork, ILogger<SavingGoalsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SavingGoalDTO> CreateSavingGoalAsync(CreateSavingGoalCommand command)
        {
            _logger.LogInformation("Creating a new saving goal with the following details: {Details}", command);

            var goal = command.ToEntity();
            await _unitOfWork.SavingGoalsRepository.AddAsync(goal);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Saving goal '{GoalName}' created successfully with ID {GoalId}", goal.GoalName, goal.Id);
            return goal.ToDto();
        }

        public async Task<SavingGoalDTO?> GetSavingGoalByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving saving goal with ID: {Id}", id);

            var goal = await _unitOfWork.SavingGoalsRepository.GetByIdAsync(id);
            if (goal == null)
            {
                _logger.LogWarning("Saving goal with ID {Id} not found.", id);
                return null;
            }

            _logger.LogInformation("Saving goal with ID {Id} retrieved successfully.", id);
            return goal != null ? goal.ToDto() : null;
        }

        public async Task<IEnumerable<SavingGoalDTO>> GetSavingGoalsByBudgetContainerIdAsync(Guid budgetContainerId)
        {
            _logger.LogInformation("Retrieving saving goals for budget container ID: {BudgetContainerId}", budgetContainerId);

            var goals = await _unitOfWork.SavingGoalsRepository.FindAsync(g => g.BudgetContainerId == budgetContainerId);

            _logger.LogInformation("Retrieved {Count} saving goals for budget container ID: {BudgetContainerId}", goals.Count(), budgetContainerId);
            return goals.Select(g => g.ToDto());
        }

        public async Task<IEnumerable<SavingGoalDTO>> GetAllSavingGoalsAsync()
        {
            _logger.LogInformation("Retrieving all saving goals.");

            var goals = await _unitOfWork.SavingGoalsRepository.GetAllAsync();
            if (goals == null || !goals.Any())
            {
                _logger.LogWarning("No saving goals found.");
                return Enumerable.Empty<SavingGoalDTO>();
            }

            _logger.LogInformation("Retrieved {Count} saving goals.", goals.Count());
            return goals.Select(g => g.ToDto());
        }

        public async Task<bool> UpdateSavingGoalAsync(UpdateSavingGoalCommand command)
        {
            _logger.LogInformation("Updating saving goal with ID: {Id}", command.Id);
            var goal = await _unitOfWork.SavingGoalsRepository.GetByIdAsync(command.Id);
            if (goal == null)
                return false;

            // Update the goal with the new data.
            goal.GoalName = command.GoalName;
            goal.TargetAmount = command.TargetAmount;
            goal.CurrentAmount = command.CurrentAmount;
            goal.TargetDate = command.TargetDate;

            var result = await _unitOfWork.SavingGoalsRepository.UpdateAsync(goal);
            if (!result)
            {
                _logger.LogWarning("Failed to update saving goal with ID: {Id}", command.Id);
                return false;
            }
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Saving goal with ID {Id} updated successfully.", command.Id);
            return result;
        }

        public async Task<bool> DeleteSavingGoalAsync(Guid id)
        {
            _logger.LogInformation("Deleting saving goal with ID: {Id}", id);
           
            var result = await _unitOfWork.SavingGoalsRepository.DeleteAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete saving goal with ID: {Id}", id);
                return false;
            }
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Saving goal with ID {Id} deleted successfully.", id);
            return result;
        }
    }
}
