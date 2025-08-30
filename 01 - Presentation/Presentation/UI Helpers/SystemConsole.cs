using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BudgetTracker.Presentation.UIHelpers
{
    public class SystemConsole : IConsole
    {
        public void Clear()
        {
            Console.Clear();
        }

        public void Write(string message) => Console.Write(message);
        public void WriteLine(string message) => Console.WriteLine(message);
        public ConsoleKeyInfo ReadKey(bool i) => Console.ReadKey(i);
        public string ReadLine() => Console.ReadLine() ?? "";
        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }
    }
}
