using Antlr4.Runtime;
using MeelanLanguage.Core;
using MeelanLanguage.Core.Grammar;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeelanLanguage.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private MeelanLanguageVisitor _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new MeelanLanguageVisitor();
        }

        [TestMethod]
        public void ItShallInterpretAProgramContainingAFunctionCallingAnotherFunction()
        {
            // Given
            var statementsContext = GivenAStatementsContext("SamplePrograms/SampleProgram1.txt");

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void ItShallInterpretAProgramContainingFunctionDefinitionsInsideFunctionDefinitions()
        {
            // Given
            var statementsContext = GivenAStatementsContext("SamplePrograms/SampleProgram2.txt");

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(3, result);
        }

        public MeelanLanguageParser.StatementsContext GivenAStatementsContext(string fileName)
        {
            var inputStream = new AntlrFileStream(fileName);
            var lexer = new MeelanLanguageLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MeelanLanguageParser(tokenStream);

            return parser.statements();
        }
    }
}