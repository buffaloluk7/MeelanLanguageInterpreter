using System;
using Antlr4.Runtime;

namespace MeelanLanguage.Core
{
    internal class ExceptionErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
            string msg, RecognitionException e)
        {
            throw new InvalidOperationException($"Invalid Expression: {msg}", e);
        }
        
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            throw new InvalidOperationException($"Invalid Expression: {msg}", e);
        }
    }
}