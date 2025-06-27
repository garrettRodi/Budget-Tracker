// File: Presentation/UIHelpers/InputProcessor.cs
using System;
using BudgetTracker.Application.Helpers;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class InputProcessor
    {
        private readonly IConsole _console;

        public InputProcessor(IConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public string GetInput(string prompt)
        {
            string input;
            do
            {
                _console.Write(prompt);
                input = _console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    _console.WriteLine("Input cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public string GetTitleInput(string prompt)
        {
            string input;
            do
            {
                _console.Write(prompt);
                input = _console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    _console.WriteLine("Input cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return TitleCaseHelper.TitleCase(input);
        }


        public DateTime GetValidDate(string prompt, bool allowFuture = false)
        {
            while (true)
            {
                _console.Write(prompt);
                var input = _console.ReadLine();
                if (!DateTime.TryParse(input, out var date))
                {
                    _console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
                    continue;
                }
                if (!allowFuture && date.Date > DateTime.Today)
                {
                    _console.WriteLine("Date cannot be in the future. Please enter a date on or before today.");
                    continue;
                }
                return date;
            }
        }

        public decimal GetValidDecimal(string prompt)
        {
            decimal result;
            while (true)
            {
                _console.Write(prompt);
                if (decimal.TryParse(_console.ReadLine(), out result))
                    return result;
                _console.WriteLine("Invalid numeric format. Please enter a valid decimal number.");
            }
        }

        public int GetValidInt(string prompt)
        {
            int result;
            while (true)
            {
                _console.Write(prompt);
                if (int.TryParse(_console.ReadLine(), out result))
                    return result;
                _console.WriteLine("Invalid numeric format. Please enter a valid number.");
            }
        }

        public TEnum GetEnum<TEnum>(string prompt) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            var validValues = Enum.GetNames(enumType);

            while (true)
            {
                _console.WriteLine(prompt);
                _console.WriteLine($"Options: {string.Join(", ", validValues)}");

                string input = GetInput("> ").Trim();

                if (Enum.TryParse<TEnum>(input, ignoreCase: true, out var result) &&
                    Enum.IsDefined(enumType, result))
                {
                    return result;
                }

                _console.WriteLine("Invalid selection. Please enter one of the listed options.");
            }
        }


        public bool GetBool(string prompt)
        {
            while (true)
            {
                _console.Write($"{prompt} (y/n): ");
                var input = _console.ReadLine().Trim().ToLower();
                if (input is "y" or "yes" or "true") return true;
                if (input is "n" or "no" or "false") return false;
                _console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            }
        }

        public Guid GetValidGuid(string prompt)
        {
            while (true)
            {
                _console.Write(prompt);
                var input = Console.ReadLine()?.Trim();
                if (Guid.TryParse(input, out var guid))
                    return guid;

                _console.WriteLine("Invalid ID format. Please enter a valid GUID.");
            }
        }
    }
}
