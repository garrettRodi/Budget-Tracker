using System;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Domain.Services
{
    public class BudgetValidator
    {
        public void ValidateBudget(BudgetContainer budget)
        {
            if (budget.StartDate < DateTime.Today)
                throw new InvalidBudgetException("Budget start date cannot be in the past.");

            if (budget.EndDate <= budget.StartDate)
                throw new InvalidBudgetException("Budget end date must be after the start date.");

            // Ensure budget has at least one BudgetItem if required.
            if (budget.Items == null || budget.Items.Count == 0)
                throw new InvalidBudgetException("Budget must contain at least one budget item.");
        }
    }
}
