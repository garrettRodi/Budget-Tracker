// File: Presentation/BudgetHelpers/BudgetHelpers.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetTracker.Application.DTOs;
using BudgetTracker.Application.DTOs.Commands;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class BudgetHelpers
    {
        private readonly IBudgetService _budgetService;
        private readonly SelectBudgetContainer _selector;
        private readonly InputProcessor _input;
        private readonly IConsole _console;
        private readonly ICategoryMappingService _categoryMappingService;
        private readonly ICurrencyService _currencyService;

        public BudgetHelpers(
            IBudgetService budgetService,
            SelectBudgetContainer selector,
            InputProcessor input,
            IConsole console,
            ICategoryMappingService categoryMappingService,
            ICurrencyService currencyService)
        {
            _budgetService = budgetService
                ?? throw new ArgumentNullException(nameof(budgetService));
            _selector = selector
                ?? throw new ArgumentNullException(nameof(selector));
            _input = input
                ?? throw new ArgumentNullException(nameof(input));
            _console = console
                ?? throw new ArgumentNullException(nameof(console));
            _categoryMappingService = categoryMappingService;
            _currencyService = currencyService;
        }

        public async Task CreateBudgetAsync()
        {
            bool create;
            do
            {
                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Create Budget ===");

                    string name = _input.GetTitleInput("Enter budget name: ");
                    var frequency = _input.GetEnum<BudgetFrequency>("Enter frequency (Weekly, Monthly, Yearly): ");
                    DateTime startDate = _input.GetValidDate("Enter start date (yyyy-MM-dd): ");
                    DateTime endDate = _input.GetValidDate("Enter end date (yyyy-MM-dd): ", allowFuture: true);
                    bool autoRenew = _input.GetBool("Auto renew? (y/n): ");
                    string nativeCurrency = _input.GetTitleInput("Enter native currency (e.g. USD, PLN): ")
                        .ToUpperInvariant();
                    decimal initialCash = _input.GetValidDecimal("Enter initial cash balance: ");
                    decimal initialBank = _input.GetValidDecimal("Enter initial bank balance: ");

                    // 1) Fetch all category names from the injected service
                    var allCategories = (await _categoryMappingService
                                             .GetAllCategoryNamesAsync())
                                        .ToList();

                    // 2) Let the user pick categories until they type 'done'
                    var items = new List<CreateBudgetItemCommand>();

                    while (true)
                    {
                        _console.Clear();
                        _console.WriteLine("=== Add Budget Items to the Budget ===");
                        _console.WriteLine("--- Select a category or type 'done' ---");
                        for (int idx = 0; idx < allCategories.Count; idx++)
                        {
                            _console.WriteLine($"{idx + 1}. {allCategories[idx]}");
                        }
                        _console.WriteLine($"{allCategories.Count + 1}. Other (enter custom)");


                        string input = _input.GetTitleInput("Choice or 'done': ").Trim();
                        _console.WriteLine($"Number {input} added to the budget items list.");
                        _console.ReadKey();

                        if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
                        {
                            if (items.Count == 0)
                            {
                                _console.WriteLine("You must enter at least one budget item before finishing.");
                                continue; // Loop again instead of returning
                            }

                            break; // Exit loop if at least one item exists
                        }

                        string category;
                        if (int.TryParse(input, out int choice)
                            && choice >= 1
                            && choice <= allCategories.Count)
                        {
                            category = allCategories[choice - 1];
                        }
                        else if (choice == allCategories.Count + 1 || !int.TryParse(input, out _))
                        {
                            category = _input.GetTitleInput("Enter custom category name: ");
                        }
                        else
                        {
                            _console.WriteLine("Invalid selection. Try again.");
                            continue;
                        }

                        // decimal plannedAmount = _input.GetValidDecimal("Enter planned amount: ");

                        items.Add(new CreateBudgetItemCommand
                        {
                            Category = category,
                            PlannedAmount = new Money(0m, _currencyService.CurrentCurrency)
                        });
                    }

                    if (items.Count == 0)
                    {
                        _console.WriteLine(
                            "You must enter at least one budget item.");
                        return;
                    }

                    var cmd = new CreateBudgetCommand
                    {
                        Name = name,
                        Frequency = frequency,
                        StartDate = startDate,
                        EndDate = endDate,
                        AutoRenew = autoRenew,
                        Currency = nativeCurrency,
                        InitialBankBalance = initialBank,
                        InitialCashBalance = initialCash,
                        Items = items
                    };

                    var dto = await _budgetService.CreateBudgetAsync(cmd);
                    _console.WriteLine(
                        $"Budget '{dto.Name}' created with ID: {dto.Id}");
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error creating budget: {ex.Message}");
                    _console.WriteLine(ex.StackTrace);
                }

                create = _input.GetBool("Do you want to create another budget? (y/n): ");
            } while (create);
        }

        public async Task ViewBudgetsAsync()
        {
            _console.Clear();
            _console.WriteLine("=== View Budgets ===");
            var budgets = await _budgetService.GetAllBudgetsAsync();
            foreach (var b in budgets)
            {
                _console.WriteLine(
                    $"ID: {b.Id} | Name: {b.Name} | Frequency: {b.Frequency} | " +
                    $"Start: {b.StartDate:yyyy-MM-dd} | End: {b.EndDate:yyyy-MM-dd} | AutoRenew: {b.AutoRenew} | NativeCurrency: {b.Currency}");
            }
            if (!budgets.Any())
                _console.WriteLine("No budgets found.");
            _console.ReadKey();
        }

        public async Task UpdateBudgetAsync()
        {
            bool update;
            do
            {
                try
                {

                    _console.Clear();
                    _console.WriteLine("=== Update Budget ===");

                    // 1. Display available budgets
                    Guid id = await _selector.GetActiveBudgetContainerIdAsync();
                    if (id == Guid.Empty)
                    {
                        _console.WriteLine("No valid budget selected.");
                        _console.ReadKey();
                        return;
                    }


                    // 2. Fetches full DTO, including existing items
                    var existing = await _budgetService.GetBudgetByIdAsync(id);
                    if (existing == null)
                    {
                        _console.WriteLine("Budget not found.");
                        _console.ReadKey();
                        return;
                    }

                    // 3. Seed a mutable list of BudgetItemCommand from the DTO’s items
                    var existingItems = existing.Items.ToList();

                    // 4. Loop until `Done` input
                    while (true)
                    {
                        _console.Clear();
                        _console.WriteLine("=== Update Budget Items ===");
                        for (int i = 0; i < existingItems.Count; i++)
                        {
                            var item = existingItems[i];
                            _console.WriteLine(
                                $"{i + 1}. {item.Category} | Planned: {item.PlannedAmount:C}");
                        }
                        _console.WriteLine("\n Choose from the following options: \n1. Add Item \n2. Edit \n3. Delete \n`Done`");
                        var input = _input.GetTitleInput("Choice: ").Trim();
                        if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
                        {
                            if (existingItems.Count == 0)
                            {
                                _console.WriteLine("You must have at least one budget item.");
                                continue; // Loop again instead of returning
                            }
                            break; // Exit loop if at least one item exists
                        }
                        switch (input)
                        {
                            case "1":
                                // Add Item
                                var category = _input.GetTitleInput("New Category: ");
                                var plannedAmount = _input.GetValidDecimal("Planned Amount: ");
                                existingItems.Add(new BudgetItemDTO
                                {
                                    Id = Guid.Empty,
                                    Category = category,
                                    PlannedAmount = plannedAmount
                                });
                                break;
                            case "2":
                                // Edit Item
                                int choice;
                                do
                                {
                                    choice = _input.GetValidInt($"Edit which item (1–{existingItems.Count}): ");
                                    if (choice < 1 || choice > existingItems.Count)
                                        _console.WriteLine($"Please enter a number between 1 and {existingItems.Count}.");
                                }
                                while (choice < 1 || choice > existingItems.Count);

                                int e = choice - 1;
                                existingItems[e].PlannedAmount = _input.GetValidDecimal($"New amount for {existingItems[e].Category}: ");
                                existingItems[e].Category = _input.GetTitleInput($"New category name ({existingItems[e].Category}): ");
                                break;
                            case "3":
                                // Delete Item
                                int delChoice;
                                do
                                {
                                    delChoice = _input.GetValidInt($"Delete which item (1–{existingItems.Count}): ");
                                    if (delChoice < 1 || delChoice > existingItems.Count)
                                        _console.WriteLine($"Please enter a number between 1 and {existingItems.Count}.");
                                }
                                while (delChoice < 1 || delChoice > existingItems.Count);

                                existingItems.RemoveAt(delChoice - 1);
                                break;
                            default:
                                _console.WriteLine("Invalid option. Please try again.");
                                continue; // Loop again instead of returning
                        }
                    }

                    // 5. Collect other budget properties
                    var name = _input.GetTitleInput($"Enter new name (current: {existing.Name}): ");
                    var frequency = _input.GetEnum<BudgetFrequency>($"Enter new frequency (current: {existing.Frequency}): ");
                    DateTime startDate = _input.GetValidDate($"Enter new start date (current: {existing.StartDate:yyyy-MM-dd}): ");
                    DateTime endDate = _input.GetValidDate($"Enter new end date (current: {existing.EndDate:yyyy-MM-dd}): ", allowFuture: true);
                    bool autoRenew = _input.GetBool($"Auto renew? (current: {existing.AutoRenew}) (y/n): ");
                    var nativeCurrency = _input.GetTitleInput($"Enter new native currency (current: {existing.Currency}): ")
                        .ToUpperInvariant();
                    decimal initialCash = _input.GetValidDecimal($"Enter new initial cash balance (current: {existing.InitialCashBalance}): ");
                    decimal initialBank = _input.GetValidDecimal($"Enter new initial bank balance (current: {existing.InitialBankBalance}): ");

                    // 6. Build & push
                    var cmd = new UpdateBudgetCommand
                    {
                        Id = id,
                        Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name,
                        Frequency = frequency,
                        StartDate = startDate,
                        EndDate = endDate,
                        AutoRenew = autoRenew,
                        Currency = nativeCurrency,
                        InitialCashBalance = initialCash,
                        InitialBankBalance = initialBank
                    };

                    bool success = await _budgetService.UpdateBudgetAsync(cmd);
                    _console.WriteLine(success
                        ? "Budget updated successfully."
                        : "Failed to update budget.");
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error updating budget: {ex.Message}");
                    _console.WriteLine(ex.StackTrace);
                }
                update = _input.GetBool("Do you want to update another budget? (y/n): ");
            } while (update);
        }
        public async Task DeleteBudgetAsync()
        {
            bool delete;
            do
            {
                try
                {
                    _console.Clear();
                    _console.WriteLine("=== Delete Budget ===");

                    var budgets = (await _budgetService.GetAllBudgetsAsync()).ToList();
                    if (!budgets.Any())
                    {
                        _console.WriteLine("No budgets available to delete.");
                        _console.ReadKey();
                        return;
                    }

                    foreach (var b in budgets)
                    {
                        _console.WriteLine($"ID: {b.Id} | Name: {b.Name} | Frequency: {b.Frequency} | Start: {b.StartDate:yyyy-MM-dd} | End: {b.EndDate:yyyy-MM-dd} | AutoRenew: {b.AutoRenew}");
                    }

                    Guid id;
                    while (true)
                    {
                        id = _input.GetValidGuid("Enter the ID of the budget to delete: ");
                        if (budgets.Any(b => b.Id == id)) break;
                        _console.WriteLine("Invalid ID. Please select one from the list above.");
                    }

                    bool success = await _budgetService.DeleteBudgetAsync(id);
                    _console.WriteLine(success ? "Budget deleted successfully." : "Failed to delete budget.");
                    _console.ReadKey();
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"Error deleting budget: {ex.Message}");
                    _console.WriteLine(ex.StackTrace);
                }
                delete = _input.GetBool("Do you want to delete another budget? (y/n): ");
            } while (delete);

        }
    }
}
