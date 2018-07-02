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
    internal sealed class ValueListField : ObjectField
    {
        public override DataFieldType FieldType => DataFieldType.ValueList;

        public override bool IsMultiple => true;

        /// <summary>
        /// 值的实际类型
        /// </summary>
        public Type ValueType
        {
            get;
            private set;
        }

        public ValueListField(PropertyRepositoryAttribute attribute)
            : base(attribute)
        {
            this.ValueType = attribute.GetElementType();
        }

        //private static DbType GetDbType(PropertyRepositoryAttribute attribute)
        //{
        //    var elementType = attribute.GetElementType();
        //    if (elementType == typeof(string)) return DbType.String;
        //    return DbType.AnsiString; //byte、int、guid等类型，都是用ansi字符串存放
        //}
    }
}
