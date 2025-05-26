using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Application.Mappers;
using BudgetTracker.Domain.Interfaces;

namespace BudgetTracker.Application.Services
{
    public class PlannedExpenseService : IPlannedExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlannedExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PlannedExpenseDTO> CreatePlannedExpenseAsync(CreatePlannedExpenseCommand cmd)
        {
            var entity = cmd.ToEntity();
            await _unitOfWork.PlannedExpenseRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity.ToDto();
        }

        public async Task<IEnumerable<PlannedExpenseDTO>> ViewPlannedExpensesAsync(Guid budgetContainerId)
        {
            var list = await _unitOfWork.PlannedExpenseRepository.ViewPlannedExpensesByBudgetAsync(budgetContainerId);
            return list.Select(pe => pe.ToDto());
        }

        public async Task<bool> UpdatePlannedExpenseAsync(UpdatePlannedExpenseCommand cmd)
        {
            var existing = await _unitOfWork.PlannedExpenseRepository.GetByIdAsync(cmd.Id);
            if (existing is null) return false;
            cmd.ToEntity(existing);
            var updated = await _unitOfWork.PlannedExpenseRepository.UpdateAsync(existing);
            if (updated) await _unitOfWork.CommitAsync();
            return updated;
        }

        public async Task<bool> DeletePlannedExpenseAsync(Guid id)
        {
            var removed = await _unitOfWork.PlannedExpenseRepository.DeleteAsync(id);
            if (removed) await _unitOfWork.CommitAsync();
            return removed;
        }
    }
}
