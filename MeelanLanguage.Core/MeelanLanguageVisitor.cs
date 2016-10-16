using System;
using System.Collections.Generic;
using System.Globalization;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Core
{
    public class MeelanLanguageVisitor : MeelanLanguageBaseVisitor<double>
    {
        public MeelanLanguageVisitor()
        {
            Functions = new Dictionary<string, MeelanLanguageParser.FuncDefContext>();
            Variables = new Dictionary<string, double>();
            TemporaryVariables = new Dictionary<string, double>();
        }

        public IDictionary<string, MeelanLanguageParser.FuncDefContext> Functions { get; }
        public IDictionary<string, double> Variables { get; }
        public IDictionary<string, double> TemporaryVariables { get; }

        public override double VisitPrint(MeelanLanguageParser.PrintContext context)
        {
            var value = Visit(context.expr());

            Console.WriteLine(value);

            return value;
        }

        public override double VisitDeclaration(MeelanLanguageParser.DeclarationContext context)
        {
            var variableName = context.ID().GetText();

            if (Variables.ContainsKey(variableName))
            {
                throw new InvalidOperationException($"A variable {variableName} has already been declared.");
            }

            var variableValue = context.expr() == null ? 0 : Visit(context.expr());
            Variables.Add(variableName, variableValue);

            return variableValue;
        }

        public override double VisitAssignment(MeelanLanguageParser.AssignmentContext context)
        {
            var variableName = context.ID().GetText();
            if (!Variables.ContainsKey(variableName))
            {
                throw new InvalidOperationException($"Variable {variableName} needs to be declared first.");
            }

            var variableValue = Visit(context.expr());
            Variables[variableName] = variableValue;

            return variableValue;
        }

        public override double VisitWhile(MeelanLanguageParser.WhileContext context)
        {
            var result = 0.0;
            while (Math.Abs(Visit(context.expr()) - 1) < double.Epsilon)
            {
                result = Visit(context.statement());
            }

            return result;
        }

        public override double VisitForIn(MeelanLanguageParser.ForInContext context)
        {
            var variableName = context.ID().GetText();
            if (TemporaryVariables.ContainsKey(variableName) || Variables.ContainsKey(variableName))
            {
                throw new InvalidOperationException($"Variable {variableName} has already been declared.");
            }

            var fromValue = ConvertStringToDouble(context.DOUBLE(0).GetText());
            var toValue = ConvertStringToDouble(context.DOUBLE(1).GetText());

            var result = 0.0;

            for (TemporaryVariables[variableName] = fromValue;
                TemporaryVariables[variableName] <= toValue;
                ++TemporaryVariables[variableName])
            {
                result = Visit(context.statement());
            }

            return result;
        }

        public override double VisitIfOptionalElse(MeelanLanguageParser.IfOptionalElseContext context)
        {
            var expressionEqualsTrue = Math.Abs(Visit(context.expr()) - 1) < double.Epsilon;
            if (expressionEqualsTrue)
            {
                return Visit(context.statement(0));
            }

            if (context.statement().Length == 2)
            {
                return Visit(context.statement(1));
            }

            return 0;
        }

        public override double VisitFuncDef(MeelanLanguageParser.FuncDefContext context)
        {
            var functionName = context.ID().GetText();
            if (Functions.ContainsKey(functionName))
            {
                throw new InvalidOperationException($"A function {functionName} has already been declared.");
            }

            Functions.Add(functionName, context);

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
            if (TemporaryVariables.ContainsKey(variableName))
            {
                return TemporaryVariables[variableName];
            }
            if (Variables.ContainsKey(variableName))
            {
                return Variables[variableName];
            }

            throw new InvalidOperationException($"Variable {variableName} needs to be declared first.");
        }

        public override double VisitFuncCall(MeelanLanguageParser.FuncCallContext context)
        {
            var functionName = context.ID().GetText();
            if (!Functions.ContainsKey(functionName))
            {
                throw new InvalidOperationException($"Function {functionName} needs to be declared first.");
            }

            var functionContext = Functions[functionName];
            var argumentNames = functionContext.idlist().ID();
            var argumentTerms = context.arglist().term();
            if (argumentNames.Length != argumentTerms.Length)
            {
                throw new InvalidOperationException(
                    $"Function needs {argumentNames.Length} arguments, but only {argumentTerms.Length} were provided.");
            }

            for (var i = 0; i < argumentNames.Length; i++)
            {
                var argumentName = argumentNames[i].GetText();
                var argumentValue = Visit(argumentTerms[i]);
                TemporaryVariables.Add(argumentName, argumentValue);
            }

            var result = Visit(functionContext.statement());

            TemporaryVariables.Clear();

            return result;
        }

        public override double VisitNumberOnly(MeelanLanguageParser.NumberOnlyContext context)
        {
            var value = context.DOUBLE().GetText();

            return ConvertStringToDouble(value);
        }

        public override double VisitBlock(MeelanLanguageParser.BlockContext context)
        {
            return Visit(context.statements());
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