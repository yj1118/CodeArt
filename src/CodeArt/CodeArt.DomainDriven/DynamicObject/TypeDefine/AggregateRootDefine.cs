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
using CodeArt.EasyMQ;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    public class AggregateRootDefine : TypeDefine,IEventHandler
    {
        /// <summary>
        /// 远程对象的原始对象被删除后，远程对象是否还保留快照信息
        /// </summary>
        public bool KeepSnapshot
        {
            get;
            private set;
        }

        /// <summary>
        /// 最低优先级的处理，先让框架更新新的对象，再出发业务事件
        /// </summary>
        public EventPriority Priority => EventPriority.Low;

        public AggregateRootDefine(string typeName, string metadataCode,bool closeMultiTenancy)
           : this(typeName, metadataCode, closeMultiTenancy, false)
        {
        }

        public AggregateRootDefine(string typeName, string metadataCode)
           : this(typeName, metadataCode, false, false)
        {
        }

        public AggregateRootDefine(string typeName, string metadataCode,bool closeMultiTenancy,bool keepSnapshot)
            : base(typeName, metadataCode, DomainObject.AggregateRootType, typeof(DynamicRoot), closeMultiTenancy)
        {
        }


        /// <summary>
        /// 该构造由框架内部调用
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="metadataCode"></param>
        /// <param name="qualifiedName"></param>
        internal AggregateRootDefine(string typeName, TypeMetadata metadata, string qualifiedName, bool closeMultiTenancy)
            : base(typeName, metadata, DomainObject.AggregateRootType, typeof(DynamicRoot), qualifiedName, closeMultiTenancy)
        {
        }

        internal protected override object GetEmptyInstance()
        {
            return new DynamicRoot(this, true);
        }

        internal override void SetBelongProperty(DomainProperty belongProperty)
        {
        }

        protected virtual void OnDeleted(object id) { }

        protected virtual void OnUpdated(object id) { }

        public void Handle(string eventName, TransferData data)
        {
            if (eventName.EndsWith("Updated"))
            {
                object id = data.Info.Dynamic.Id;
                OnUpdated(id);
                return;
            }

            if (eventName.EndsWith("Deleted"))
            {
                object id = data.Info.Dynamic.Id;
                OnDeleted(id);
                return;
            }
        }
    }
    

    public class AggregateRootDefine<T> : AggregateRootDefine
        where T : TypeDefine
    {
        public AggregateRootDefine(string typeName, string metadataCode,bool closeMultiTenancy)
            : base(typeName, metadataCode, closeMultiTenancy)
        {

        }

        public AggregateRootDefine(string typeName, string metadataCode)
            : base(typeName, metadataCode, false)
        {

        }

        public static object Empty
        {
            get
            {
                return TypeDefine.GetDefine<T>().EmptyInstance;
            }
        }
    }
}
