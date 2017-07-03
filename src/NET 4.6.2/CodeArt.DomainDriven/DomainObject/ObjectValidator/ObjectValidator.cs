using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    public abstract class ObjectValidator<T> : IObjectValidator
        where T : class, IDomainObject
    {
        public void Validate(IDomainObject obj, ValidationResult result)
        {
            T target = obj as T;
            if (target == null) throw new TypeMismatchException(obj.GetType(), typeof(T));
            Validate(target, result);
        }

        protected abstract void Validate(T obj, ValidationResult result);
    }
}
