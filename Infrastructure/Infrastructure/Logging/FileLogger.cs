using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BudgetTracker.Infrastructure.Interface;

namespace BudgetTracker.Infrastructure.Logging
{
    public class FileLogger : IBudgetLogger
    {
        private readonly string _filePath;
        
        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            File.AppendAllText(_filePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
