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

        public DateTime GetValidDate (string prompt)
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
        // You can add more methods here to process different types of input (e.g., dates, decimals).
    }
}
