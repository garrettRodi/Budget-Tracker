using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Interfaces;
using BudgetTracker.Application.Mappers;
using System.Runtime.CompilerServices;
using BudgetTracker.Domain.Services;

namespace BudgetTracker.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        // (Optionally, you can later inject CurrencyConversionService if multi‑currency support is needed.)

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExpenseDTO> CreateExpenseAsync(CreateExpenseCommand createCommand)
        {
            Expense entity = createCommand.ToEntity();

            // Validate the expense before saving.
            var validator = new ExpenseValidator();
            validator.ValidateExpense(entity);

            await _unitOfWork.ExpenseRepository.AddAsync(entity);
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
