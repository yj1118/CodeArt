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
    /// 领域对象中引用对象的字段
    /// </summary>
    internal sealed class EntityObjectField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.EntityObject;

        public override bool IsMultiple => false;


        public EntityObjectField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }



    }
}
