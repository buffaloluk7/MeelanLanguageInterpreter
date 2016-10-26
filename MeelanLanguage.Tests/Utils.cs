using Antlr4.Runtime;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Tests
{
    public static class Utils
    {
        public static class Given
        {
            public static MeelanLanguageParser.StatementsContext GivenAStatementsContextForProgramCode(
                string programCode)
            {
                var inputStream = new AntlrInputStream(programCode);

                return GivenAStatementsContext(inputStream);
            }

            public static MeelanLanguageParser.StatementsContext GivenAStatementsContextForFileContent(string fileName)
            {
                var inputStream = new AntlrFileStream(fileName);

                return GivenAStatementsContext(inputStream);
            }

            private static MeelanLanguageParser.StatementsContext GivenAStatementsContext(ICharStream inputStream)
            {
                var lexer = new MeelanLanguageLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new MeelanLanguageParser(tokenStream);

                return parser.statements();
            }
        }
    }
}