using System.Runtime.CompilerServices;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;

namespace BudgetTracker.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseDTO> CreateExpenseAsync(CreateExpenseCommand createCommand);
        Task<IEnumerable<ExpenseDTO>> GetExpenseAsync();
        Task<ExpenseDTO?> GetExpenseByIdAsync(Guid id);
        Task<bool> UpdateExpenseAsync(UpdateExpenseCommand updateCommand);
        Task<bool> DeleteExpenseAsync(Guid id);
    }
}
