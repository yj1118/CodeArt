using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 领域对象中值对象的字段
    /// </summary>
    internal sealed class ValueObjectField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.ValueObject;

        public override bool IsMultiple => false;


        public ValueObjectField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }
    }
}
