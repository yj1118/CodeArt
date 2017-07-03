using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CodeArt.DTO;
using CodeArt.Runtime;

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
        /// 根据远程类型的全称获取本地动态类型的定义
        /// </summary>
        /// <param name="remoteTypeFullName"></param>
        /// <returns></returns>
        public static TypeDefine GetDefine(string remoteTypeFullName)
        {
            TypeDefine define = null;
            if (_defineIndex.TryGetValue(remoteTypeFullName, out define)) return define;
            throw new DomainDrivenException(string.Format(Strings.NoDyanmicTypeDefineForRemoteType, remoteTypeFullName));
        }

        private static ConcurrentDictionary<string, TypeDefine> _defineIndex = new ConcurrentDictionary<string, TypeDefine>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeFullName">远程类型的全称</param>
        /// <param name="define"></param>
        internal static void AddDefineIndex(string remoteTypeFullName, TypeDefine define)
        {
            _defineIndex.TryAdd(remoteTypeFullName, define);
        }


        /// <summary>
        /// 在程序启动的时候分析所有远程类型的定义，记录信息
        /// </summary>
        public static void Initialize()
        {
            var types = AssemblyUtil.GetTypesByAttribute<RemoteTypeAttribute>();
            foreach (var type in types)
            {
                TypeDefine.Initialize(type);
            }
        }


    }
}