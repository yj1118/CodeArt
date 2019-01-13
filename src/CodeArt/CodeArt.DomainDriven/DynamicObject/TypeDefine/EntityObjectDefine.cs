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
    internal class EntityObjectDefine : TypeDefine
    {
        public EntityObjectDefine(string typeName, string metadataCode,bool closeMultiTenancy)
            : base(typeName, metadataCode, DomainObject.EntityObjectType, typeof(DynamicEntity), closeMultiTenancy)
        {
        }


        /// <summary>
        /// 该构造由框架内部调用
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="metadataCode"></param>
        /// <param name="qualifiedName"></param>
        internal EntityObjectDefine(string typeName, TypeMetadata metadata, string qualifiedName, bool closeMultiTenancy)
            : base(typeName, metadata, DomainObject.EntityObjectType, typeof(DynamicEntity), qualifiedName, closeMultiTenancy)
        {
        }

        internal protected override object GetEmptyInstance()
        {
            return new DynamicEntity(this, true);
        }

        internal override void SetBelongProperty(DomainProperty belongProperty)
        {
            
        }

    }
}
