using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelSpecExtractor
{
    public static class ConsoleExtensions
    {
        public static void PrintConsoleWarning(int lineNumber, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Line {0} - Warning!... {1}", lineNumber, message);
            Console.ResetColor();
        }

        public static void PrintConsoleMessage(int lineNumber, string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Line {0} - {1}", lineNumber, message);
            Console.ResetColor();
        }

    }
}
