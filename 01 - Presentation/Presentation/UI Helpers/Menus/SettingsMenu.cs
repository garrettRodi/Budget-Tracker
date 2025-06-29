using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetTracker.Application.Interfaces;
using BudgetTracker.Presentation.PresentationHelpers;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class SettingsMenu
    {
        private readonly IConsole _console;
        private readonly InputProcessor _input;
        private readonly SettingsHelpers _settingsHelpers;
        private readonly ICurrencyService _currencyService;

        public SettingsMenu (InputProcessor input, IConsole console, SettingsHelpers settingsHelpers, ICurrencyService currencyService)
        {
            _settingsHelpers = settingsHelpers ?? throw new ArgumentNullException(nameof(settingsHelpers));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        }
            
        public async Task ViewSettingsAsync()
        {
            bool back = false;
            while (!back)
            {
                _console.Clear();
                _console.WriteLine("=== Settings ===\n");
                _console.WriteLine("1. Select Language");
                _console.WriteLine("2. Select Currency");
                _console.WriteLine("3. Return to Main Menu");

                var input = _input.GetValidDecimal("Enter your choice:");

                switch (input)
                {
                    case 1: _console.WriteLine("Language Selections are limited to English. Try again later.");
                        _console.ReadKey();
                        break;
                    case 2: _console.WriteLine($"1. Change currency (Current: {_currencyService.CurrentCurrency})");
                        await _settingsHelpers.SelectCurrencyAsync();
                        break;
                    case 3: _console.WriteLine("Return to Main Menu"); 
                        back = true;
                        _console.ReadKey();
                        break;
                    default:
                        _console.WriteLine("Invalid choice. Please select a valid menu option.");
                        break;
                }
            }
        }
    }
}
