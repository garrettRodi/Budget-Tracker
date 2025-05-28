using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Application.Helpers
{
    public static class CategoryHelper
    {
        /// <summary>
        /// Normalizes a category string to Title Case (first letter uppercase, rest lowercase).
        /// </summary>
        public static string NormalizeCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return string.Empty;
            category = category.Trim();
            return char.ToUpper(category[0]) + category.Substring(1).ToLower();
        }
    }
}
