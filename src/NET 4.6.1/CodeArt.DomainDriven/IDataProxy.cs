using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.DomainDriven
{
    public interface IDataProxy : INotNullObject
    {
        object Load(DomainProperty property);

        void Save(DomainProperty property, object value);

        /// <summary>
        /// 属性的数据是否已被加载
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        bool IsLoaded(DomainProperty property);


        DomainObject Owner { get; set; }

        /// <summary>
        /// 拷贝数据代理中的数据
        /// </summary>
        /// <param name="target"></param>
        void Copy(IDataProxy target);
    }
}
