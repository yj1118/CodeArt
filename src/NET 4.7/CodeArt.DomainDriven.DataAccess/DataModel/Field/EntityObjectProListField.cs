using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 领域对象中多个高级引用对象的字段
    /// </summary>
    internal sealed class EntityObjectProListField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.EntityObjectProList;

        public override bool IsMultiple => true;

        public EntityObjectProListField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }
    }
}
