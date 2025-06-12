using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Presentation.UIHelpers
{
    // Abstraction for console I/O, so you can substitute other UIs or fakes.
    public interface IConsole
    {
        void Write(string message);
        void WriteLine(string message = "");
        string ReadLine();
        ConsoleKeyInfo ReadKey(bool intercept = false);
        void Clear();
        ConsoleColor ForegroundColor { get; set; }
    }
}
