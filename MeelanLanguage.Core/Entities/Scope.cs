using System.Collections.Generic;
using MeelanLanguage.Core.Grammar;

namespace MeelanLanguage.Core.Entities
{
    public class Scope<T>
    {
        public Scope(Scope<T> callingScope)
        {
            CallingScope = callingScope;
            Functions = new Dictionary<string, MeelanLanguageParser.FuncDefContext>();
            Variables = new Dictionary<string, T>();
        }

        public Scope<T> CallingScope { get; }

        public IDictionary<string, MeelanLanguageParser.FuncDefContext> Functions { get; }

        public IDictionary<string, T> Variables { get; }

        public bool HasFunctionDeclared(string functionName)
        {
            return Functions.ContainsKey(functionName);
        }

        public MeelanLanguageParser.FuncDefContext GetFunction(string functionName)
        {
            return Functions[functionName];
        }

        public void SetFunction(string functionName, MeelanLanguageParser.FuncDefContext functionContext)
        {
            Functions[functionName] = functionContext;
        }

        public bool HasVariableDeclared(string variableName)
        {
            return Variables.ContainsKey(variableName);
        }

        public T GetVariable(string variableName)
        {
            return Variables[variableName];
        }

        public void SetVariable(string variableName, T variableValue)
        {
            Variables[variableName] = variableValue;
        }
    }
}