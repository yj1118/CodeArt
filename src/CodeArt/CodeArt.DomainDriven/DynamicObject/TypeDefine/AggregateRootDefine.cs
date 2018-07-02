using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    public class AggregateRootDefine : TypeDefine
    {
        public AggregateRootDefine(string typeName, string metadataCode)
            : base(typeName, metadataCode, DomainObject.AggregateRootType, typeof(DynamicRoot))
        {
        }


        /// <summary>
        /// 该构造由框架内部调用
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="metadataCode"></param>
        /// <param name="qualifiedName"></param>
        internal AggregateRootDefine(string typeName, TypeMetadata metadata, string qualifiedName)
            : base(typeName, metadata, DomainObject.AggregateRootType, typeof(DynamicRoot), qualifiedName)
        {
        }

        internal protected override object GetEmptyInstance()
        {
            return new DynamicRoot(this, true);
        }

        internal override void SetBelongProperty(DomainProperty belongProperty)
        {
        }

    }
}
