using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;


namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 普通值的字段信息
    /// </summary>
    internal class ValueField : DataField
    {
        public override DataFieldType FieldType
        {
            get
            {
                return DataFieldType.Value;
            }
        }

        public override bool IsMultiple => false;

        public ValueField(PropertyRepositoryAttribute attribute, params DbFieldType[] dbFieldTypes)
            : base(attribute, Util.GetDbType(attribute.IsEmptyable ? attribute.EmptyableValueType : attribute.PropertyType), dbFieldTypes)
        {
            ValidateType(attribute);
        }

        private void ValidateType(PropertyRepositoryAttribute attribute)
        {
            if (!DataUtil.IsPrimitiveType(attribute.PropertyType) && !attribute.IsEmptyable)
            {
                throw new DomainDesignException(Strings.DomainObjectTypeWrong);
            }
        }
    }

}
