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
    internal class ValueObjectDefine : TypeDefine
    {
        public ValueObjectDefine(string typeName, string metadataCode, bool closeMultiTenancy)
            : base(typeName, metadataCode, DomainObject.ValueObjectType, typeof(DynamicValueObject), closeMultiTenancy)
        {
        }

        /// <summary>
        /// 该构造由框架内部调用
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="metadataCode"></param>
        /// <param name="qualifiedName"></param>
        internal ValueObjectDefine(string typeName, TypeMetadata metadata, string qualifiedName, bool closeMultiTenancy)
            : base(typeName, metadata, DomainObject.ValueObjectType, typeof(DynamicValueObject), qualifiedName, closeMultiTenancy)
        {
        }

        internal protected override object GetEmptyInstance()
        {
            return new DynamicValueObject(this, true);
        }

        internal override void SetBelongProperty(DomainProperty belongProperty)
        {
        }

    }
}
