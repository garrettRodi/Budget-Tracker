// File: Presentation/UIHelpers/InputProcessor.cs
using System;

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
            _console.Write(prompt);
            return _console.ReadLine() ?? string.Empty;
        }

        public DateTime GetValidDate(string prompt)
        {
            DateTime date;
            while (true)
            {
                _console.Write(prompt);
                if (DateTime.TryParse(_console.ReadLine(), out date))
                    return date;
                _console.WriteLine("Invalid date format. Please use yyyy-mm-dd.");
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

        public T GetEnum<T>(string prompt, T defaultValue) where T : struct, Enum
        {
            while (true)
            {
                _console.Write($"{prompt} [{defaultValue}]: ");
                var input = _console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    return defaultValue;
                if (Enum.TryParse<T>(input, true, out var result))
                    return result;
                _console.WriteLine($"Invalid. Choose from: {string.Join(", ", Enum.GetNames(typeof(T)))}");
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
                Console.Write(prompt);
                var input = Console.ReadLine()?.Trim();
                if (Guid.TryParse(input, out var guid))
                    return guid;

                Console.WriteLine("Invalid ID format. Please enter a valid GUID.");
            }
        }
    }
}
