using System;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.Interfaces;

namespace BudgetTracker.Domain.Services
{
    public class BudgetValidator : IValidator<BudgetContainer>
    {
        /// <summary>
        /// Validates a budget container.
        /// For new budgets, set isNew to true to enforce that start date is not in the past.
        /// For updates, you might allow an earlier start date.
        /// </summary>
       
        public void ValidateBudget(BudgetContainer budget, bool isNew = true)
        {
            if (isNew && budget.StartDate < DateTime.Today)
                throw new InvalidBudgetException("Budget start date cannot be in the past.");

            if (budget.EndDate <= budget.StartDate)
                throw new InvalidBudgetException("Budget end date must be after the start date.");

            if (budget.BudgetItems == null || budget.BudgetItems.Count == 0)
                throw new InvalidBudgetException("Budget must contain at least one budget item.");
        }

        public void Validate(BudgetContainer budget)
        {
            ValidateBudget(budget, isNew: true);
        }
    }
}
