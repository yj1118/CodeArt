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
        [ConstructorRepository()]
        public DynamicValueObject(TypeDefine define, bool isEmpty)
            : base(define, isEmpty)
        {
            this.OnConstructed();
        }

        /// <summary>
        /// 由于ORM存储中间表时需要用到编号，所以我们提供了该属性
        /// 该属性仅在ORM中使用，不要在领域层中出现
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get;
            private set;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TrySetId(Guid id)
        {
            if (this.Id == default(Guid))
                this.Id = id; //如果没有编号，那么值对象需要追加编号，有编号则意味着值对象在数据库中已存在
        }
    }
}