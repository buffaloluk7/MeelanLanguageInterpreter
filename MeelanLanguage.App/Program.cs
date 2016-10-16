using System;
using MeelanLanguage.Core;

namespace MeelanLanguage.App
{
    internal class Program
    {
        private static readonly MeelanLanguageInterpreter Interpreter = new MeelanLanguageInterpreter();

        private static void Main(string[] args)
        {
            Console.Write("Input: ");

            string programCode;
            while ((programCode = Console.ReadLine()) != "exit")
            {
                try
                {
                    var result = Interpreter.InterpretProgramCodeFromString(programCode);
                    Console.WriteLine($"Result: {result}");
                }
                catch (InvalidOperationException exception)
                {
                    Console.WriteLine($"\tError while interpreting the program code.\n\t{exception.Message}");
                }

                Console.Write("Input: ");
            }
        }
    }
}