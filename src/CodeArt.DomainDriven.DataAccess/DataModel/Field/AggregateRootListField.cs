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
    /// 内聚根集合
    /// </summary>
    internal sealed class AggregateRootListField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.AggregateRootList;

        public override bool IsMultiple => true;

        public AggregateRootListField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }
    }
}
