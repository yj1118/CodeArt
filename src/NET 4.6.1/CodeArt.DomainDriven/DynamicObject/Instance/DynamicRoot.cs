using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;

using CodeArt.Util;
using CodeArt.DTO;


namespace CodeArt.DomainDriven
{
    public sealed class DynamicRoot : DynamicEntity, IAggregateRoot
    {
        public DynamicRoot(TypeDefine define, bool isEmpty)
            : base(define, isEmpty)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public DynamicRoot()
        {
            this.OnConstructed();
        }
    }
}