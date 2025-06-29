using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.UIHelpers;

namespace BudgetTracker.Presentation.PresentationHelpers
{
    public class SettingsHelpers
    {
        private readonly InputProcessor _input;
            private readonly IConsole _console;
        private readonly ICurrencyService _currencyService;

        public SettingsHelpers(InputProcessor input, IConsole console, ICurrencyService currencyService)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        }

        public async Task SelectCurrencyAsync()
        {
            _console.Clear();
            _console.WriteLine("=== Select Currency ===");
            string code = _input.GetTitleInput("Enter currency code (i.e. USD, PLN):")
                .ToUpperInvariant();
            await _currencyService.SetCurrencyAsync(code);

            _console.WriteLine($"Currency switched to: {code}");
            _console.WriteLine($"Conversion rate (1 USD → {code}):");
            _console.WriteLine("Press Enter to continue...");
            _console.ReadLine();
        }
    }
}
