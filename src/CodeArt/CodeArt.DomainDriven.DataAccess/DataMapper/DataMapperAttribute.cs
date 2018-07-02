using CodeArt.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven.DataAccess
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataMapperAttribute : Attribute
    {
        public Type DataMapperType
        {
            get;
            private set;
        }

        public DataMapperAttribute(Type dataMapperType)
        {
            this.DataMapperType = dataMapperType;
        }

        /// <summary>
        /// 得到仓储定义的数据映射器的类型
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        internal static Type GetDataMapperType(IRepository repository)
        {
            var attribute = AttributeUtil.GetAttribute<DataMapperAttribute>(repository.GetType());
            if (attribute == null) return null;
            return attribute.DataMapperType;

        }

    }
}
