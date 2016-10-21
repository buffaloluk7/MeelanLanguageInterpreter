using System;
using System.Collections.Generic;
using System.Globalization;
using MeelanLanguage.Core.Entities;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Core
{
    public class MeelanLanguageVisitor : MeelanLanguageBaseVisitor<double>
    {
        private readonly CallStack<double> _callStack;

        public MeelanLanguageVisitor()
        {
            _callStack = new CallStack<double>();
        }

        public Scope<double> CurrentScope => _callStack.CurrentScope;

        public override double VisitPrint(MeelanLanguageParser.PrintContext context)
        {
            var value = Visit(context.expr());
            Console.WriteLine(value);

            return value;
        }

        public override double VisitDeclaration(MeelanLanguageParser.DeclarationContext context)
        {
            var variableName = context.ID().GetText();
            if (_callStack.CurrentScope.HasVariableDeclared(variableName))
            {
                throw new InvalidOperationException(
                    $"A variable {variableName} has already been declared in this scope.");
            }

            var variableValue = context.expr() == null ? 0 : Visit(context.expr());
            _callStack.CurrentScope.SetVariable(variableName, variableValue);

            return variableValue;
        }

        public override double VisitAssignment(MeelanLanguageParser.AssignmentContext context)
        {
            var variableName = context.ID().GetText();
            var scopeContainingVariableDeclaration = _callStack.FindScopeForVariableName(variableName);
            if (scopeContainingVariableDeclaration == null)
            {
                throw new InvalidOperationException($"Variable {variableName} needs to be declared first.");
            }

            var variableValue = Visit(context.expr());
            scopeContainingVariableDeclaration.SetVariable(variableName, variableValue);

            return variableValue;
        }

        public override double VisitWhile(MeelanLanguageParser.WhileContext context)
        {
            var result = 0.0;
            while (Math.Abs(Visit(context.expr()) - 1) < double.Epsilon)
            {
                _callStack.CreateScope();
                result = Visit(context.statement());
                _callStack.RemoveCurrentScope();
            }

            return result;
        }

        public override double VisitForIn(MeelanLanguageParser.ForInContext context)
        {
            _callStack.CreateScope();

            var variableName = context.ID().GetText();
            if (_callStack.CurrentScope.HasVariableDeclared(variableName))
            {
                throw new InvalidOperationException($"Variable {variableName} has already been declared in this scope.");
            }

            var fromValue = ConvertStringToDouble(context.DOUBLE(0).GetText());
            var toValue = ConvertStringToDouble(context.DOUBLE(1).GetText());
            var result = 0.0;


            for (var i = fromValue; i <= toValue; ++i)
            {
                _callStack.CreateScope();

                _callStack.CurrentScope.SetVariable(variableName, i);
                result = Visit(context.statement());

                _callStack.RemoveCurrentScope();
            }

            _callStack.RemoveCurrentScope();

            return result;
        }

        public override double VisitIfOptionalElse(MeelanLanguageParser.IfOptionalElseContext context)
        {
            var expressionEqualsTrue = Math.Abs(Visit(context.expr()) - 1) < double.Epsilon;
            var result = 0.0;

            if (expressionEqualsTrue)
            {
                _callStack.CreateScope();
                result = Visit(context.statement(0));
                _callStack.RemoveCurrentScope();
            }

            if (context.statement().Length == 2)
            {
                _callStack.CreateScope();
                result = Visit(context.statement(1));
                _callStack.RemoveCurrentScope();
            }

            return result;
        }

        public override double VisitFuncDef(MeelanLanguageParser.FuncDefContext context)
        {
            var functionName = context.ID().GetText();
            if (_callStack.CurrentScope.HasFunctionDeclared(functionName))
            {
                throw new InvalidOperationException(
                    $"A function {functionName} has already been declared in this scope.");
            }

            _callStack.CurrentScope.SetFunction(functionName, context);

            return 0;
        }

        public override double VisitCmp(MeelanLanguageParser.CmpContext context)
        {
            var leftValue = Visit(context.sum(0));
            if (context.sum().Length == 1)
            {
                return leftValue;
            }

            var rightValue = Visit(context.sum(1));
            var equalsSign = context.@operator.Text;

            switch (equalsSign)
            {
                case "<":
                    return leftValue < rightValue ? 1 : 0;
                case "=<":
                    return leftValue <= rightValue ? 1 : 0;
                case "==":
                    return Math.Abs(leftValue - rightValue) < double.Epsilon ? 1 : 0;
                case "><":
                    return Math.Abs(leftValue - rightValue) > double.Epsilon ? 1 : 0;
                case ">=":
                    return leftValue >= rightValue ? 1 : 0;
                case ">":
                    return leftValue > rightValue ? 1 : 0;
                default:
                    throw new InvalidOperationException($"Invalid equals sign {equalsSign} at VisitCmp.");
            }
        }

        public override double VisitSum(MeelanLanguageParser.SumContext context)
        {
            var leftValue = Visit(context.product(0));
            if (context.product().Length == 1)
            {
                return leftValue;
            }

            var rightValue = Visit(context.product(1));
            var @operator = context.@operator.Text;

            switch (@operator)
            {
                case "+":
                    return leftValue + rightValue;
                case "-":
                    return leftValue - rightValue;
                default:
                    throw new InvalidOperationException($"Invalid operator {@operator} at VisitSum.");
            }
        }

        public override double VisitProduct(MeelanLanguageParser.ProductContext context)
        {
            var leftValue = Visit(context.unary(0));
            if (context.unary().Length == 1)
            {
                return leftValue;
            }

            var rightValue = Visit(context.unary(1));
            var @operator = context.@operator.Text;

            switch (@operator)
            {
                case "*":
                    return leftValue*rightValue;
                case "/":
                    return leftValue/rightValue;
                case "%":
                    return leftValue%rightValue;
                default:
                    throw new InvalidOperationException($"Invalid operator {@operator} at VisitProduct.");
            }
        }

        public override double VisitUnaryTerm(MeelanLanguageParser.UnaryTermContext context)
        {
            var value = Visit(context.unary());

            return value*-1;
        }

        public override double VisitTermInBraces(MeelanLanguageParser.TermInBracesContext context)
        {
            return Visit(context.expr());
        }

        public override double VisitVariableOnly(MeelanLanguageParser.VariableOnlyContext context)
        {
            var variableName = context.ID().GetText();
            var scopeContainingVariableDeclaration = _callStack.FindScopeForVariableName(variableName);
            if (scopeContainingVariableDeclaration == null)
            {
                throw new InvalidOperationException($"Variable {variableName} needs to be declared first.");
            }

            return scopeContainingVariableDeclaration.GetVariable(variableName);
        }

        public override double VisitFuncCall(MeelanLanguageParser.FuncCallContext context)
        {
            var functionName = context.ID().GetText();
            var scopeContainingFunction = _callStack.FindScopeForFunctionName(functionName);
            if (scopeContainingFunction == null)
            {
                throw new InvalidOperationException($"Function {functionName} needs to be declared first.");
            }

            var functionContext = scopeContainingFunction.GetFunction(functionName);
            var argumentNames = functionContext.idlist().ID();
            var argumentTerms = context.arglist().term();
            if (argumentNames.Length != argumentTerms.Length)
            {
                throw new InvalidOperationException(
                    $"Function needs {argumentNames.Length} arguments, but only {argumentTerms.Length} were provided.");
            }

            // Look for argument values in current scope before creating a new one
            var arguments = new Dictionary<string, double>();
            for (var i = 0; i < argumentNames.Length; i++)
            {
                var argumentName = argumentNames[i].GetText();
                var argumentValue = Visit(argumentTerms[i]);
                arguments.Add(argumentName, argumentValue);
            }

            // Add argument values to newly created scope
            _callStack.CreateScope(scopeContainingFunction);

            foreach (var argument in arguments)
            {
                _callStack.CurrentScope.SetVariable(argument.Key, argument.Value);
            }

            var result = Visit(functionContext.statement());

            _callStack.RemoveCurrentScope();

            return result;
        }

        public override double VisitNumberOnly(MeelanLanguageParser.NumberOnlyContext context)
        {
            var value = context.DOUBLE().GetText();

            return ConvertStringToDouble(value);
        }

        public override double VisitBlock(MeelanLanguageParser.BlockContext context)
        {
            _callStack.CreateScope();

            var value = Visit(context.statements());

            _callStack.RemoveCurrentScope();

            return value;
        }

        public override double VisitIfRequiredElse(MeelanLanguageParser.IfRequiredElseContext context)
        {
            var expressionEqualsTrue = Math.Abs(Visit(context.expr(0)) - 1) < double.Epsilon;

            return Visit(expressionEqualsTrue
                ? context.expr(1)
                : context.expr(2));
        }

        private static double ConvertStringToDouble(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}