using MeelanLanguage.Core;
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
            var statementsContext =
                Utils.Given.GivenAStatementsContextForFileContent("SamplePrograms/SampleProgram1.txt");

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void ItShallInterpretAProgramContainingFunctionDefinitionsInsideFunctionDefinitions()
        {
            // Given
            var statementsContext =
                Utils.Given.GivenAStatementsContextForFileContent("SamplePrograms/SampleProgram2.txt");

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void ItShallInterpretAProgramCallingAFunctionRepeatinglyUsingWhileLoop()
        {
            // Given
            var statementsContext =
                Utils.Given.GivenAStatementsContextForFileContent("SamplePrograms/SampleProgram3.txt");
            var callingStatementsContext = Utils.Given.GivenAStatementsContextForProgramCode("addUsingWhile(10, 32)");

            _sut.Visit(statementsContext);
            var result = _sut.Visit(callingStatementsContext);

            // Then
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ItShallInterpretAProgramCallingAFunctionRepeatinglyUsingForInLoop()
        {
            // Given
            var statementsContext =
                Utils.Given.GivenAStatementsContextForFileContent("SamplePrograms/SampleProgram3.txt");
            var callingStatementsContext = Utils.Given.GivenAStatementsContextForProgramCode("addUsingForIn(10, 32)");

            _sut.Visit(statementsContext);
            var result = _sut.Visit(callingStatementsContext);

            // Then
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ItShallInterpretAProgramCallingAFunctionRepeatinglyUsingRecursion()
        {
            // Given
            var statementsContext =
                Utils.Given.GivenAStatementsContextForFileContent("SamplePrograms/SampleProgram3.txt");
            var callingStatementsContext = Utils.Given.GivenAStatementsContextForProgramCode("addUsingRecursion(10, 32)");

            _sut.Visit(statementsContext);
            var result = _sut.Visit(callingStatementsContext);

            // Then
            Assert.AreEqual(42, result);
        }
    }
}