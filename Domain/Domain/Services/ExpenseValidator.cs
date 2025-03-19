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

            // Additional business rules can be validated here.
            if (expense.ExpenseDate > DateTime.Now)
                throw new Exception("Expense date cannot be in the future.");
        }
    }
}
