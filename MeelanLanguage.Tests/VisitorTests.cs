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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var declarationStatementsContext = GivenAStatementsContext(declarationProgramCode);
            const string whileProgramCode = "while i > 2 do i = i - 1";
            var whileStatementsContext = GivenAStatementsContext(whileProgramCode);

            // When
            _sut.Visit(declarationStatementsContext);
            _sut.Visit(whileStatementsContext);

            // Then
            Assert.AreEqual(2, _sut.CurrentScope.GetVariable("i"));
        }

        [TestMethod]
        public void ItShallInterpretAForInLoop()
        {
            // Given
            const string programCode = "var sum = 0; var index = 1; for index in 1...10 { sum = sum + index }";
            var statementsContext = GivenAStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(55, _sut.CurrentScope.GetVariable("sum"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenTryingToAccessAnUndeclaredVariable()
        {
            // Given
            const string assignmentProgramCode = "if i > 5 then 1";
            var statementsContext = GivenAStatementsContext(assignmentProgramCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.IsTrue(_sut.CurrentScope.HasVariableDeclared("i"));
        }

        [TestMethod]
        public void ItShallInterpretAVariableDeclarationWithAnAssignment()
        {
            // Given
            const string programCode = "var i2 = 5";
            var statementsContext = GivenAStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(5, _sut.CurrentScope.GetVariable("i2"));
        }

        [TestMethod]
        public void ItShallInterpretAnAssignment()
        {
            // Given
            const string declarationProgramCode = "var i = 42";
            var declarationStatementsContext = GivenAStatementsContext(declarationProgramCode);
            const string assignmentProgramCode = "i = 24";
            var assignmentStatementsContext = GivenAStatementsContext(assignmentProgramCode);

            // When
            _sut.Visit(declarationStatementsContext);
            _sut.Visit(assignmentStatementsContext);

            // Then
            Assert.AreEqual(24, _sut.CurrentScope.GetVariable("i"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenAssigningAValueToAnUndeclaredVariable()
        {
            // Given
            const string assignmentProgramCode = "i = 24";
            var statementsContext = GivenAStatementsContext(assignmentProgramCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

            // When
            _sut.Visit(statementsContext);

            // Then
            Assert.IsTrue(_sut.CurrentScope.HasFunctionDeclared("calculate"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenDeclaringAFunctionWithTheSameNameTwice()
        {
            // Given
            const string programCode = "funcdef calculate() { 4 + 2 }";
            var statementsContext = GivenAStatementsContext(programCode);

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
            var statementsContext = GivenAStatementsContext(programCode);

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
            var funcDefStatementsContext = GivenAStatementsContext(funcDefProgramCode);
            const string funcCallProgramCode = "theAnswerToLifeTheUniverseAndEverything()";
            var funcCallStatementsContext = GivenAStatementsContext(funcCallProgramCode);

            // When
            _sut.Visit(funcDefStatementsContext);
            var result = _sut.Visit(funcCallStatementsContext);

            // Then
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ItShallIgnoreInlineComments()
        {
            // Given
            const string programCode = "var a = 42 // test";
            var statementsContext = GivenAStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void ItShallIgnoreMultilineComments()
        {
            // Given
            const string programCode = "var /* test */ b = 42";
            var statementsContext = GivenAStatementsContext(programCode);

            // When
            var result = _sut.Visit(statementsContext);

            // Then
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItShallThrowAnInvalidOperationExceptionWhenTooLittleFunctionArgumentsAreProvided()
        {
            // Given
            const string funcDefProgramCode =
                "funcdef calculate(leftOperand, rightOperand) { leftOperand + rightOperand }";
            var funcDefStatementsContext = GivenAStatementsContext(funcDefProgramCode);
            const string funcCallProgramCode = "calculate(4)";
            var funcCallStatementsContext = GivenAStatementsContext(funcCallProgramCode);

            // When
            _sut.Visit(funcDefStatementsContext);
            _sut.Visit(funcCallStatementsContext);

            // Then
            // throw an InvalidOperationException
        }

        public void ItShallInterpretAFunctionCallWithTwoArguments()
        {
            // Given
            const string funcDefProgramCode =
                "funcdef calculate(leftOperand, rightOperand) { leftOperand + rightOperand }";
            var funcDefStatementsContext = GivenAStatementsContext(funcDefProgramCode);
            const string funcCallProgramCode = "calculate(4, 5)";
            var funcCallStatementsContext = GivenAStatementsContext(funcCallProgramCode);

            // When
            _sut.Visit(funcDefStatementsContext);
            var result = _sut.Visit(funcCallStatementsContext);

            // Then
            Assert.AreEqual(9, result);
        }

        public MeelanLanguageParser.StatementsContext GivenAStatementsContext(string programCode)
        {
            var inputStream = new AntlrInputStream(programCode);
            var lexer = new MeelanLanguageLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MeelanLanguageParser(tokenStream);

            return parser.statements();
        }
    }
}