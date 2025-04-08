using System;
using System.Linq;                      
using System.Collections.Generic;       
using System.Threading.Tasks;          
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.Interfaces;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class BudgetSelector
    {
        private readonly IBudgetService _budgetService;

        public BudgetSelector(IBudgetService budgetService)
        {
            // Throw exception if budgetService is null to catch issues early
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
        }

        /// <summary>
        /// Retrieves the active BudgetContainer ID.
        /// If only one container exists, returns that ID.
        /// If multiple exist, prompts the user to select one.
        /// If no containers exist, returns Guid.Empty.
        /// </summary>
        public async Task<Guid> GetActiveBudgetContainerIdAsync()
        {
            List<BudgetDTO> budgetList = (await _budgetService.GetAllBudgetsAsync()).ToList();

            if (budgetList.Count == 0)
            {
                Console.WriteLine("No budget containers available. Please create a budget first.");
                return Guid.Empty;
            }
            else if (budgetList.Count == 1)
            {
                // Only one container exists; return its ID.
                return budgetList.First().Id;
            }
            else
            {
                // More than one container exists; prompt the user to select one.
                Console.WriteLine("Multiple budget containers found. Please select one:");
                for (int i = 0; i < budgetList.Count; i++)
                {
                    var budget = budgetList[i];
                    Console.WriteLine($"{i + 1}. {budget.Name} (ID: {budget.Id})");
                }

                int selection = 0;
                while (!int.TryParse(Console.ReadLine(), out selection) || selection < 1 || selection > budgetList.Count)
                {
                    Console.Write("Invalid selection. Please enter a valid number: ");
                }

                return budgetList[selection - 1].Id;
            }
        }
    }
}
