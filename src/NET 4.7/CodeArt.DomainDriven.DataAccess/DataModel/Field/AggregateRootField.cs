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
    /// 领域对象中根对象的字段
    /// </summary>
    internal sealed class AggregateRootField : ObjectField
    {

        public override DataFieldType FieldType
        {
            get
            {
                return DataFieldType.AggregateRoot;
            }
        }

        public override bool IsMultiple => false;


        public AggregateRootField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
        }
    }
}
