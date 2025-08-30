using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;

namespace BudgetTracker.Application.Interfaces
{
    public interface IIncomeService
    {
        Task<IncomeDTO> CreateIncomeAsync(CreateIncomeCommand createCommand);
        Task<IEnumerable<IncomeDTO>> GetAllIncomesAsync();
        Task<IncomeDTO?> GetIncomeByIdAsync(Guid id);
        Task<IEnumerable<IncomeDTO>> GetIncomesByBudgetContainerIdAsync(Guid budgetContainerId);
        Task<bool> UpdateIncomeAsync(UpdateIncomeCommand updateCommand);
        Task<bool> DeleteIncomeAsync(Guid id);
    }
}
