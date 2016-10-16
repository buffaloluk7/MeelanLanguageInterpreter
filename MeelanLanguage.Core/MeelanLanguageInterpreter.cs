using Antlr4.Runtime;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Core
{
    public class MeelanLanguageInterpreter
    {
        private readonly MeelanLanguageVisitor _visitor = new MeelanLanguageVisitor();

        public double InterpretProgramCodeFromFilePath(string filePath)
        {
            var inputStream = new AntlrFileStream(filePath);
            var result = InterpretProgram(inputStream);

            return result;
        }

        public double InterpretProgramCodeFromString(string programCode)
        {
            var inputStream = new AntlrInputStream(programCode);
            var result = InterpretProgram(inputStream);

            return result;
        }

        private double InterpretProgram(ICharStream inputStream)
        {
            var parser = SetupParser(inputStream);
            var statementsContext = parser.statements();
            var result = _visitor.Visit(statementsContext);

            return result;
        }

        private static MeelanLanguageParser SetupParser(ICharStream inputStream)
        {
            var lexer = new MeelanLanguageLexer(inputStream);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ExceptionErrorListener());

            var tokenStream = new CommonTokenStream(lexer);

            var parser = new MeelanLanguageParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ExceptionErrorListener());

            return parser;
        }
    }
}