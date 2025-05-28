using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Helpers
{
    public static class TitleCaseHelper
    {
        /// <summary>
        /// Normalizes a category string to Title Case (first letter uppercase, rest lowercase).
        /// </summary>
        public static string TitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            input = input.Trim();
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}
