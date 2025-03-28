using System;
using System.Xml;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class InputProcessor
    {
        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public DateTime GetValidDate(string prompt)
        {
            DateTime date;
            while (true)
            {
                Console.WriteLine(prompt);
                if (DateTime.TryParse(Console.ReadLine(), out date))
                    return date;
                Console.WriteLine("Invalid date format. Please use yyyy-mm-dd.");
            }
        }

        public decimal GetValidDecimal(string prompt)
        {
            decimal result;
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out result))
                    return result;
                Console.WriteLine("Invalid numeric format. Please enter a valid decimal number.");
            }
        }

        public T GetEnum<T>(string prompt, T defaultValue) where T : struct, Enum
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                if (Enum.TryParse<T>(input, true, out T result))
                {
                    return result;
                }
                else
                {
                    Console.WriteLine($"Invalid value. Please enter one of the following: {string.Join(", ", Enum.GetNames(typeof(T)))}");
                    // Optionally, you can return defaultValue after a few attempts.
                }
            }
        }

        public bool GetBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";
                if (input == "y" || input == "yes" || input == "true")
                    return true;
                if (input == "n" || input == "no" || input == "false")
                    return false;

                Console.WriteLine("Invalid input. Please enter 'yes' or 'no' (or true/false).");
            }
        }

        public int GetValidInt(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out result))
                    return result;
                Console.WriteLine("Invalid numeric format. Please enter a valid decimal number.");
                
            }
        }

    // You can add more methods here to process different types of input (e.g., dates, decimals).
    }
}
