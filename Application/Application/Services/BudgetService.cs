using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Domain.Services;
using BudgetTracker.Infrastructure.Interface;


namespace BudgetTracker.Application.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBudgetLogger _logger;

        public BudgetService(IUnitOfWork unitOfWork, IBudgetLogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Creates a new budget based on user-provided details.
        public async Task<BudgetDTO> CreateBudgetAsync(CreateBudgetCommand command)
        {
            try
            {
                // Convert the command to a BudgetContainer entity using the mapper.
                var budget = command.ToEntity();
                var validator = new BudgetValidator();
                validator.ValidateBudget(budget);

                // Persist the new budget.
                await _unitOfWork.BudgetRepository.AddAsync(budget);
                await _unitOfWork.CommitAsync();
                return budget.ToDto();
            }
            catch (BudgetTrackerException ex)
            {
                _logger.Log($"Budget creation failed: {ex.Message}");
                throw new ApplicationException("Failed to create budget. Please review your input.", ex);
            }
        }

        // Retrieves a budget by its unique identifier.
        public async Task<BudgetDTO?> GetBudgetByIdAsync(Guid id)
        {
            var budget = await _unitOfWork.BudgetRepository.GetByIdAsync(id);
            return budget != null ? budget.ToDto() : null;
        }

        // Retrieves all persisted budgets.
        public async Task<IEnumerable<BudgetDTO>> GetAllBudgetsAsync()
        {
            var budgets = await _unitOfWork.BudgetRepository.GetAllAsync();
            return budgets.Select(b => b.ToDto());
        }

        // Updates an existing budget using user-provided details.
        public async Task<bool> UpdateBudgetAsync(UpdateBudgetCommand command)
        {
            try
            {
                var budget = await _unitOfWork.BudgetRepository.GetByIdAsync(command.Id);
                if (budget == null)
                    return false;
                var validator = new BudgetValidator();
                validator.ValidateBudget(budget);

                // Update budget properties from the command.
                budget.Name = command.Name;
                budget.Frequency = command.Frequency;
                budget.StartDate = command.StartDate;
                budget.EndDate = command.EndDate;
                budget.AutoRenew = command.AutoRenew;

                var result = await _unitOfWork.BudgetRepository.UpdateAsync(budget);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (BudgetTrackerException ex)
            {
                _logger.Log($"Budget update failed: {ex.Message}");
                throw new ApplicationException("Failed to update budget. Please review your input.", ex);
            }
        }

        // Deletes a budget based on its ID.
        public async Task<bool> DeleteBudgetAsync(Guid id)
        {
            var result = await _unitOfWork.BudgetRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return result;
        }
    }
}
