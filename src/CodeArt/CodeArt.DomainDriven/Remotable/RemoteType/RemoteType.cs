using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CodeArt.EasyMQ;
using CodeArt.EasyMQ.Event;

using CodeArt.DTO;
using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 远程类型的契约
    /// </summary>
    [DebuggerDisplay("FullName = {FullName}")]
    public sealed class RemoteType
    {
        /// <summary>
        /// 远程类型的命名空间
        /// </summary>
        public string Namespace
        {
            get;
            private set;
        }

        /// <summary>
        /// 远程类型的名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public string FullName
        {
            get;
            private set;
        }

        internal RemoteType(string ns, string typeName)
        {
            this.Namespace = ns;
            this.Name = typeName;
            this.FullName = string.IsNullOrEmpty(this.Namespace) ? this.Name : string.Format("{0}.{1}", this.Namespace, this.Name);
            _hashCode = this.FullName.GetHashCode();
        }

        private int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            var target = obj as RemoteType;
            if (target == null) return false;
            return target.GetHashCode() == this.GetHashCode();
        }


        /// <summary>
        /// 根据远程类型的全称获取本地动态类型的定义，本地可以定义多个动态类型
        /// </summary>
        /// <param name="remoteTypeFullName"></param>
        /// <returns></returns>
        public static IEnumerable<TypeDefine> GetDefines(string remoteTypeFullName)
        {
            return _defineIndex.GetValues(remoteTypeFullName);
        }

        private static MultiDictionary<string, TypeDefine> _defineIndex = new MultiDictionary<string, TypeDefine>(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName">远程类型的全称</param>
        /// <param name="define"></param>
        internal static void AddDefineIndex(string remoteTypeFullName, TypeDefine define)
        {
            _defineIndex.TryAdd(remoteTypeFullName, define, TypeDefineComparer.Instance);
        }

        private class TypeDefineComparer : IEqualityComparer<TypeDefine>
        {
            public bool Equals(TypeDefine x, TypeDefine y)
            {
                return x.TypeName == y.TypeName;
            }

            public int GetHashCode(TypeDefine obj)
            {
                return obj.TypeName.GetHashCode();
            }

            public static readonly TypeDefineComparer Instance = new TypeDefineComparer();

        }


        /// <summary>
        /// 在程序启动的时候分析所有远程类型的定义，记录信息
        /// </summary>
        public static void Initialize()
        {
            _types = new List<RemoteType>();
            var defineTypes = AssemblyUtil.GetTypesByAttribute<RemoteTypeAttribute>();
            foreach (var defineType in defineTypes)
            {
                if (TypeDefine.IsIgnore(defineType)) continue;
                var obj = (TypeDefine)TypeDefine.Initialize(defineType);
                _types.Add(obj.RemoteType);
            }

            foreach (var defineType in defineTypes)
            {
                SubscribeRemoteObjectEvent(defineType);
            }
        }


        private static void SubscribeRemoteObjectEvent(Type defineType)
        {
            var define = TypeDefine.GetDefine(defineType);
            var handler = define as IEventHandler;
            if (handler == null) return;

            var updateEventName = RemoteObjectUpdated.GetEventName(define.RemoteType);
            var deleteEventName = RemoteObjectDeleted.GetEventName(define.RemoteType);
            EventPortal.Subscribe(updateEventName, handler);
            EventPortal.Subscribe(deleteEventName, handler); //这里订阅了事件，但是不需要对应的有取消订阅的事件，因为这些事件都是框架本身就已经订阅了的，当程序关闭时会自动取消订阅
        }


        private static List<RemoteType> _types;

        /// <summary>
        /// 获得当前应用程序定义的所有远程类型的定义
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<RemoteType> GetTypes()
        {
            return _types;
        }


    }
}