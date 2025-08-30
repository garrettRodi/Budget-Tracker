using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Domain.Services;
using BudgetTracker.Domain.ValueObjects;
using Microsoft.Extensions.Logging; // Use Microsoft.Extensions.Logging for ILogger<T>


namespace BudgetTracker.Application.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BudgetService> _logger;
        private readonly ICurrencyService _currencyService;
        

        public BudgetService(IUnitOfWork unitOfWork, ILogger<BudgetService> logger, ICurrencyService currencyService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currencyService = currencyService;
        }

        // Creates a new budget based on user-provided details.
        public async Task<BudgetDTO> CreateBudgetAsync(CreateBudgetCommand command)
        {
            try
            {
                // Convert the command to a BudgetContainer entity using the mapper.
                var budget = command.ToEntity(_currencyService.CurrentCurrency);
                var validator = new BudgetValidator();
                validator.ValidateBudget(budget, isNew: true);

                // Persist the new budget.
                await _unitOfWork.BudgetRepository.AddAsync(budget);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Budget '{BudgetName}' created successfully with ID {BudgetId}", budget.Name, budget.Id);
                return budget.ToDto();
            }
            catch (BudgetTrackerException ex)
            {
                _logger.LogError($"Budget creation failed: {ex.Message}");
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
                // 1. Load the budget by ID.
                var budget = await _unitOfWork.BudgetRepository.GetByIdAsync(command.Id);
                if (budget == null)
                    return false;

                // 2. Update budget properties
                budget.Name = command.Name;
                budget.Frequency = command.Frequency;
                budget.StartDate = command.StartDate;
                budget.EndDate = command.EndDate;
                budget.AutoRenew = command.AutoRenew;

                budget.InitialCashBalance = new Money(command.InitialCashBalance, _currencyService.CurrentCurrency);
                budget.InitialBankBalance = new Money(command.InitialBankBalance, _currencyService.CurrentCurrency);

                // 3. Reconcile items
                // Remove
                var toRemove = budget.BudgetItems
                    .Where(e => !command.Items.Any(i => i.Id == e.Id))
                    .ToList();
                foreach (var item in toRemove)
                {
                    budget.RemoveItem(item.Id);
                }
                // Add or update
                foreach (var itemDto in command.Items)
                {
                    var existing = budget.BudgetItems.FirstOrDefault(e => e.Id == itemDto.Id);
                    if (existing != null)
                    {
                        existing.Category = itemDto.Category;
                        existing.PlannedAmount = new Money(itemDto.PlannedAmount, _currencyService.CurrentCurrency);
                    }
                    else
                    {
                        budget.AddItem(
                            Guid.NewGuid(),
                            itemDto.Category,
                            new Money(itemDto.PlannedAmount, _currencyService.CurrentCurrency));
                    }
                }
                // 4. Validate & commit
                var validator = new BudgetValidator();
                validator.ValidateBudget(budget, isNew: false);

                var success = await _unitOfWork.BudgetRepository.UpdateAsync(budget);
                await _unitOfWork.CommitAsync();
                return success;
            }
            catch (BudgetTrackerException ex)
            {
                _logger.LogError($"Budget update failed: {ex.Message}");
                throw new ApplicationException("Failed to update budget. Please review your input.", ex);
            }
        }

        // Deletes a budget based on its ID.
        public async Task<bool> DeleteBudgetAsync(Guid id)
        {
            var result = await _unitOfWork.BudgetRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            if (result)
            {
                _logger.LogInformation("Budget with ID {BudgetId} deleted successfully", id);
            }
            else
            {
                _logger.LogWarning("Budget with ID {BudgetId} not found for deletion", id);
            }
            return result;
        }
    }
}
