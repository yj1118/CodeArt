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
    /// 领域对象中高级引用对象的字段
    /// </summary>
    internal sealed class EntityObjectProField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.EntityObjectPro;

        public override bool IsMultiple => false;

        public EntityObjectProField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }



    }
}
