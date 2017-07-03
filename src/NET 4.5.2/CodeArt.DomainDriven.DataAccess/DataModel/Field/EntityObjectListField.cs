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
    /// 领域对象中实体对象集合的字段
    /// </summary>
    internal sealed class EntityObjectListField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.EntityObjectList;

        public override bool IsMultiple => true;

        public EntityObjectListField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }
    }
}
