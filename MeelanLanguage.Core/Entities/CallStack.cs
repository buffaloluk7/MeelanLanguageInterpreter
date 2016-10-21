namespace MeelanLanguage.Core.Entities
{
    internal class CallStack<T>
    {
        public CallStack()
        {
            CurrentScope = new Scope<T>(null, null);
        }

        public Scope<T> CurrentScope { get; private set; }

        public Scope<T> FindScopeForFunctionName(string functionName)
        {
            return FindScopeForFunctionName(functionName, CurrentScope);
        }

        private static Scope<T> FindScopeForFunctionName(string functionName, Scope<T> scope)
        {
            if (scope?.HasFunctionDeclared(functionName) ?? true)
            {
                return scope;
            }

            return FindScopeForFunctionName(functionName, scope.ParentScope);
        }

        public Scope<T> FindScopeForVariableName(string variableName)
        {
            return FindScopeForVariableName(variableName, CurrentScope);
        }

        private static Scope<T> FindScopeForVariableName(string variableName, Scope<T> scope)
        {
            if (scope?.HasVariableDeclared(variableName) ?? true)
            {
                return scope;
            }

            return FindScopeForVariableName(variableName, scope.ParentScope);
        }

        public void CreateScope(Scope<T> parentScope = null)
        {
            CurrentScope = new Scope<T>(parentScope ?? CurrentScope, CurrentScope);
        }

        public void RemoveCurrentScope()
        {
            CurrentScope = CurrentScope.CallingScope;
        }
    }
}