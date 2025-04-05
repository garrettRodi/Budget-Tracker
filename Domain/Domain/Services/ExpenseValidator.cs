using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Domain.Exceptions;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Domain.Services
{
    public class ExpenseValidator
    {
        public void ValidateExpense(Expense expense)
        {
            if (expense.Amount < 0)
                throw new InvalidExpenseException(expense.Amount);

            if (expense.ExpenseDate > DateTime.Now)
                throw new Exception("Expense date cannot be in the future.");

            if (string.IsNullOrWhiteSpace(expense.Name))
                throw new Exception("Expense name cannot be empty.");

            if (string.IsNullOrWhiteSpace(expense.Category))
                throw  new Exception("Expense category cannot be empty.");

            // Add more validation rules as needed.
        }
    }
}
