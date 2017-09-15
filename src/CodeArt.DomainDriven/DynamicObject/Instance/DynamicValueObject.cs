using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using System.ComponentModel;

using CodeArt.Util;
using CodeArt.DTO;


namespace CodeArt.DomainDriven
{
    public class DynamicValueObject : DynamicObject, IValueObject
    {
        public DynamicValueObject(TypeDefine define, bool isEmpty)
            : base(define, isEmpty)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public DynamicValueObject()
        {
            this.OnConstructed();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Guid Id
        {
            get;
            private set;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void SetId(Guid id)
        {
            if (this.Id == default(Guid))
            {
                this.Id = id; //如果没有编号，那么值对象需要追加编号，有编号则意味着值对象在数据库中已存在
            }
        }
    }
}