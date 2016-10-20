using System.Collections.Generic;
using System.Linq;

namespace MeelanLanguage.Core.Entities
{
    internal class CallStack<T>
    {
        private readonly IList<Scope<T>> _callStack;

        public CallStack()
        {
            CurrentScope = new Scope<T>(null);
            _callStack = new List<Scope<T>> {CurrentScope};
        }

        public Scope<T> CurrentScope { get; private set; }

        public Scope<T> FindScopeForFunctionName(string functionName)
        {
            return _callStack
                .SkipWhile(scope => scope != CurrentScope)
                .FirstOrDefault(scope => scope.HasFunctionDeclared(functionName));
        }

        public Scope<T> FindScopeForVariableName(string variableName)
        {
            var currentScopeIndex = _callStack.IndexOf(CurrentScope);

            return _callStack
                .Skip(currentScopeIndex)
                .FirstOrDefault(scope => scope.HasVariableDeclared(variableName));
        }

        public void CreateScope(Scope<T> callingScope = null)
        {
            var scope = new Scope<T>(callingScope ?? CurrentScope);
            var newScopeIndex = _callStack.IndexOf(CurrentScope);

            _callStack.Insert(newScopeIndex, scope);
            CurrentScope = scope;
        }

        public void RemoveCurrentScope()
        {
            var newScopeIndex = CurrentScope.CallingScope != null
                ? _callStack.IndexOf(CurrentScope.CallingScope) - 1
                : 0;

            _callStack.Remove(CurrentScope);

            CurrentScope = _callStack.ElementAt(newScopeIndex);
        }
    }
}