using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace CodeArt.Runtime.IL
{
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        public IVariable Declare(Type type)
        {
            return Declare(type, false);
        }

        public IVariable Declare(Type type,string name)
        {
            return Declare(type, false, name);
        }

        public IVariable Declare(Type type, bool isPinned)
        {
            return _currentScope.DeclareLocal(type, isPinned);
        }

        public IVariable Declare(Type type, bool isPinned,string name)
        {
            return _currentScope.DeclareLocal(type, isPinned, name);
        }

        public IVariable Declare<T>()
        {
            return Declare(typeof(T));
        }

        public IVariable Declare<T>(string name)
        {
            return Declare(typeof(T),name);
        }

        public IVariable Declare<T>(bool isPinned)
        {
            return Declare(typeof(T), isPinned);
        }

        public IVariable Declare(int value)
        {
            var temp = this.Declare<int>();
            this.Assign(temp, () =>
            {
                this.Load(value);
            });
            return temp;
        }

        public IVariable Declare(string value)
        {
            var temp = this.Declare<string>();
            this.Assign(temp, () =>
            {
                this.Load(value);
            });
            return temp;
        }

        public IVariable Declare(bool value)
        {
            var temp = this.Declare<bool>();
            this.Assign(temp, () =>
            {
                this.Load(value);
            });
            return temp;
        }
    }
}
