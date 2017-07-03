using System;
using System.Linq;
using System.Reflection;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>Encapsulates a object that can builds <see cref="IVariable"/> instances.</para>
    /// </summary>
    public interface IExpression : IVariable
    {
        IExpression Copy();
        IExpression AddMember(string memberName);
        IExpression AddMember(PropertyInfo property);
        IExpression AddMember(FieldInfo field);
        IExpression MakeReadOnly();
        bool IsReadOnly { get; }
    }
}
