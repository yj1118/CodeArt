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
    /// 领域对象中值对象集合的字段
    /// </summary>
    internal sealed class ValueObjectListField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.ValueObjectList;

        public override bool IsMultiple => true;

        public ValueObjectListField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }
    }
}
