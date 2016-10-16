using System;
using System.Collections.Generic;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Core
{
    public class MeelanLanguageVisitor : MeelanLanguageBaseVisitor<int>
    {
        public MeelanLanguageVisitor()
        {
            Functions = new Dictionary<string, MeelanLanguageParser.FuncDefContext>();
            Variables = new Dictionary<string, int>();
            TemporaryVariables = new Dictionary<string, int>();
        }

        public IDictionary<string, MeelanLanguageParser.FuncDefContext> Functions { get; }
        public IDictionary<string, int> Variables { get; }
        public IDictionary<string, int> TemporaryVariables { get; }

        public override int VisitPrint(MeelanLanguageParser.PrintContext context)
        {
            var value = Visit(context.expr());

            Console.WriteLine(value);

            return value;
        }

        public override int VisitDeclaration(MeelanLanguageParser.DeclarationContext context)
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

        public override int VisitAssignment(MeelanLanguageParser.AssignmentContext context)
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

        public override int VisitWhile(MeelanLanguageParser.WhileContext context)
        {
            while (Visit(context.expr()) == 1)
                Visit(context.statement());

            return 0;
        }

        public override int VisitIfElse(MeelanLanguageParser.IfElseContext context)
        {
            var expressionEqualsTrue = Visit(context.expr()) == 1;
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

        public override int VisitFuncDef(MeelanLanguageParser.FuncDefContext context)
        {
            var functionName = context.ID().GetText();
            if (Functions.ContainsKey(functionName))
            {
                throw new InvalidOperationException($"A function {functionName} has already been declared.");
            }

            Functions.Add(functionName, context);

            return 0;
        }

        public override int VisitCmp(MeelanLanguageParser.CmpContext context)
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
                case "=<": // ?
                    return leftValue <= rightValue ? 1 : 0;
                case "==":
                    return leftValue == rightValue ? 1 : 0;
                case "><":
                    return leftValue != rightValue ? 1 : 0;
                case ">=":
                    return leftValue >= rightValue ? 1 : 0;
                case ">":
                    return leftValue > rightValue ? 1 : 0;
                default:
                    throw new InvalidOperationException($"Invalid equals sign {equalsSign} at VisitCmp.");
            }
        }

        public override int VisitSum(MeelanLanguageParser.SumContext context)
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

        public override int VisitProduct(MeelanLanguageParser.ProductContext context)
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

        public override int VisitUnaryTerm(MeelanLanguageParser.UnaryTermContext context)
        {
            var value = Visit(context.unary());

            return value*-1;
        }

        public override int VisitTermInBraces(MeelanLanguageParser.TermInBracesContext context)
        {
            return Visit(context.expr());
        }

        public override int VisitVariableOnly(MeelanLanguageParser.VariableOnlyContext context)
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

        public override int VisitFuncCall(MeelanLanguageParser.FuncCallContext context)
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

        public override int VisitNumberOnly(MeelanLanguageParser.NumberOnlyContext context)
        {
            var value = context.INT().GetText();

            return int.Parse(value);
        }

        public override int VisitBlock(MeelanLanguageParser.BlockContext context)
        {
            return Visit(context.statements());
        }
    }
}