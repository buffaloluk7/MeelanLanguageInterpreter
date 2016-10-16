using System;
using MeelanLanguage.Core;

namespace MeelanLanguage.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var programCode = Console.ReadLine();

            var interpreter = new MeelanLanguageInterpreter();
            var result = interpreter.InterpretProgramCodeFromString(programCode);

            Console.WriteLine(result);
        }
    }
}