// File: Presentation/UIHelpers/SystemConsole.cs
using System;

namespace BudgetTracker.Presentation.UIHelpers
{
    // Production implementation that directly delegates to System.Console.
    public class SystemConsole : IConsole
    {
        public void Write(string message) => Console.Write(message);
        public void WriteLine(string message = "") => Console.WriteLine(message);
        public string ReadLine() => Console.ReadLine() ?? "";
        public ConsoleKeyInfo ReadKey(bool intercept = false)
            => Console.ReadKey(intercept);
    }
}
