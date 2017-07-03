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
    /// 值集合的字段
    /// </summary>
    internal sealed class ValueListField : DataField
    {
        public override DataFieldType FieldType => DataFieldType.ValueList;

        public override bool IsMultiple => false;

        public ValueListField(PropertyRepositoryAttribute attribute)
            : base(attribute, DbType.String, Array.Empty<DbFieldType>())
        {
        }
    }
}
