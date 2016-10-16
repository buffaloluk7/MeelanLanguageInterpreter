using System;
using Antlr4.Runtime;
using MeelanLanguage.Core;
using MeelanLanguage.Core.Grammar;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MeelanLanguage.Tests
{
    [TestClass]
    public class VisitorTests
    {
        private MeelanLanguageVisitor _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new MeelanLanguageVisitor();
        }

        [TestMethod]
        public void ItShallInterpretASingleNumber()
        {
            // Given
            const string programCode = "7";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void ItShallInterpretAnAdditionOfTwoNumbers()
        {
            // Given
            const string programCode = "3+ 4";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void ItShallInterpretAnSubtractionOfTwoNumbersWrappedInBraces()
        {
            // Given
            const string programCode = "(12-15)";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(-3, result);
        }

        [TestMethod]
        public void ItShallInterpretAnUnaryOperationOfANegativeNumberWrappedInBraces()
        {
            // Given
            const string programCode = "-(-13-15)";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(28, result);
        }

        [TestMethod]
        public void ItShallInterpretAComparisonOfTwoTerms()
        {
            // Given
            const string programCode = "(14 - 12) < (12 + 3)";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ItShallInterpretAnIfStatementWithoutAnElseBranch()
        {
            // Given
            const string programCode = "if (4 < 5) then 9 + 2";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void ItShallInterpretAnIfStatementWithAnElseBranch()
        {
            // Given
            const string programCode = "if (4 == 5) then 9 + 2 else 13";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(13, result);
        }

        [TestMethod]
        public void ItShallInterpretNestedIfStatements()
        {
            // Given
            const string programCode = "if (3 % 2 == 0) then 9 + 2 else { if 7 + 2 == 9 then {1} else 2 }";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ItShallInterpretAWhileLoop()
        {
            // Given
            const string declarationProgramCode = "var i = 5";
            var declarationStatementsContext = GivenAnStatementsContext(declarationProgramCode);
            const string whileProgramCode = "while i > 2 do i = i - 1";
            var whileStatementsContext = GivenAnStatementsContext(whileProgramCode);

            // When
            _sut.Visit(declarationStatementsContext);
            _sut.Visit(whileStatementsContext);

            // Then
            Assert.AreEqual(2, _sut.Variables["i"]);
        }

        [TestMethod]
        public void ItShallInterpretAForInLoop()
        {
            // Given
            const string programCode = "var sum = 0; for index in 1...10 { sum = sum + index }";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(55, _sut.Variables["sum"]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenTryingToAccessAnUndeclaredVariable()
        {
            // Given
            const string assignmentProgramCode = "if i > 5 then 1";
            var statementsContext = GivenAnStatementsContext(assignmentProgramCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            // throw an InvalidOperationException
        }

        [TestMethod]
        public void ItShallInterpretAVariableDeclarationWithoutAnAssignment()
        {
            // Given
            const string programCode = "var i";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.IsTrue(_sut.Variables.ContainsKey("i"));
        }

        [TestMethod]
        public void ItShallInterpretAVariableDeclarationWithAnAssignment()
        {
            // Given
            const string programCode = "var i2 = 5";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(5, _sut.Variables["i2"]);
        }

        [TestMethod]
        public void ItShallInterpretAnAssignment()
        {
            // Given
            const string declarationProgramCode = "var i = 42";
            var declarationStatementsContext = GivenAnStatementsContext(declarationProgramCode);
            const string assignmentProgramCode = "i = 24";
            var assignmentStatementsContext = GivenAnStatementsContext(assignmentProgramCode);

            // When
            _sut.Visit(declarationStatementsContext);
            _sut.Visit(assignmentStatementsContext);

            // Then
            Assert.AreEqual(24, _sut.Variables["i"]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenAssigningAValueToAnUndeclaredVariable()
        {
            // Given
            const string assignmentProgramCode = "i = 24";
            var statementsContext = GivenAnStatementsContext(assignmentProgramCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            // throw an InvalidOperationException
        }

        [TestMethod]
        public void ItShallInterpretAFunctionDeclaration()
        {
            // Given
            const string programCode = "funcdef calculate() { 4 + 2 }";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.IsTrue(_sut.Functions.ContainsKey("calculate"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenDeclaringAFunctionWithTheSameNameTwice()
        {
            // Given
            const string programCode = "funcdef calculate() { 4 + 2 }";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);
            _sut.Visit(statementsContext);

            // Then
            // throw an InvalidOperationException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenCallingAnUndeclaredFunction()
        {
            // Given
            const string programCode = "calculate() { 4 + 2 }";
            var statementsContext = GivenAnStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            // throw an InvalidOperationException
        }

        [TestMethod]
        public void ItShallInterpretAFunctionCallWithoutArguments()
        {
            // Given
            const string funcDefProgramCode = "funcdef theAnswerToLifeTheUniverseAndEverything() { 44 - 2 }";
            var funcDefStatementsContext = GivenAnStatementsContext(funcDefProgramCode);
            const string funcCallProgramCode = "theAnswerToLifeTheUniverseAndEverything()";
            var funcCallStatementsContext = GivenAnStatementsContext(funcCallProgramCode);

            // When
            _sut.Visit(funcDefStatementsContext);
            var result = _sut.Visit(funcCallStatementsContext);

            // Then
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ItShallInterpretAFunctionCallWithTwoArguments()
        {
            // Given
            const string funcDefProgramCode =
                "funcdef calculate(leftOperand, rightOperand) { leftOperand + rightOperand }";
            var funcDefStatementsContext = GivenAnStatementsContext(funcDefProgramCode);
            const string funcCallProgramCode = "calculate(4, 5)";
            var funcCallStatementsContext = GivenAnStatementsContext(funcCallProgramCode);

            // When
            _sut.Visit(funcDefStatementsContext);
            var result = _sut.Visit(funcCallStatementsContext);

            // Then
            Assert.AreEqual(9, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenTooLittleFunctionArgumentsAreProvided()
        {
            // Given
            const string funcDefProgramCode =
                "funcdef calculate(leftOperand, rightOperand) { leftOperand + rightOperand }";
            var funcDefStatementsContext = GivenAnStatementsContext(funcDefProgramCode);
            const string funcCallProgramCode = "calculate(4)";
            var funcCallStatementsContext = GivenAnStatementsContext(funcCallProgramCode);

            // When
            _sut.Visit(funcDefStatementsContext);
            _sut.Visit(funcCallStatementsContext);

            // Then
            // throw an InvalidOperationException
        }

        public MeelanLanguageParser.StatementsContext GivenAnStatementsContext(string programCode)
        {
            var inputStream = new AntlrInputStream(programCode);
            var lexer = new MeelanLanguageLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MeelanLanguageParser(tokenStream);

            return parser.statements();
        }
    }
}