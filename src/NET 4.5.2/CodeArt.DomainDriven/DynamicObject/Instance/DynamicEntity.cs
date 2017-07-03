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
    public class DynamicEntity : DynamicObject, IEntityObject
    {
        public DynamicEntity(TypeDefine define, bool isEmpty)
            : base(define, isEmpty)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public DynamicEntity()
        {
            this.OnConstructed();
        }

        public object GetIdentity()
        {
            var property = DomainProperty.GetProperty(this.Define.MetadataType, EntityObject.IdPropertyName);
            return this.GetValue(property);
        }
    }
}