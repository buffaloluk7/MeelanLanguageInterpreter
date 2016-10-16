﻿using Antlr4.Runtime;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Core
{
    // https://github.com/tunnelvisionlabs/antlr4cs
    // https://github.com/tunnelvisionlabs/antlr4cs/wiki
    // file encoding! Unicode (UTF-8 without signature) - Codepage 65001
    // see: https://github.com/ckuhn203/Antlr4Calculator/ for a good math calculator implementation
    public class MeelanLanguageInterpreter
    {
        public int InterpretProgramCodeFromFilePath(string filePath)
        {
            var inputStream = new AntlrFileStream(filePath);
            var parser = SetupParser(inputStream);
            var result = ParseProgram(parser);

            return result;
        }

        public int InterpretProgramCodeFromString(string programCode)
        {
            var inputStream = new AntlrInputStream(programCode);
            var parser = SetupParser(inputStream);
            var result = ParseProgram(parser);

            return result;
        }

        private static int ParseProgram(MeelanLanguageParser parser)
        {
            var statementsContext = parser.statements();
            var visitor = new MeelanLanguageVisitor();
            var result = visitor.Visit(statementsContext);

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