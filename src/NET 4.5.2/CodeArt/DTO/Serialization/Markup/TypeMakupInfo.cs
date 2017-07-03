using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DTO
{
    /// <summary>
    /// 基于标记的信息
    /// </summary>
    [DebuggerDisplay("{ClassType.Name}")]
    internal class TypeMakupInfo : TypeSerializationInfo
    {
        public TypeMakupInfo(Type classType)
            : base(classType)
        {
            this.Initialize();
        }


        protected override DTOClassAttribute GetClassAttribute(Type classType)
        {
            return DTOClassAttribute.GetAttribute(classType);
        }

        protected override DTOMemberAttribute GetMemberAttribute(MemberInfo member)
        {
            return DTOMemberAttribute.GetAttribute(member);
        }

        protected override void OnBuildMembers(List<MemberSerializationInfo> members)
        {
            //什么都不用执行
        }


        public override void Serialize(object instance, DTObject dto)
        {
            using (var temp = _writerPool.Borrow())
            {
                var writer = temp.Item;
                writer.Initialize(dto);
                SerializeMethod(instance, writer);
            }
        }


        public override void Deserialize(object instance, DTObject dto)
        {
            using (var temp = _readerPool.Borrow())
            {
                var reader = temp.Item;
                reader.Initialize(dto);
                DeserializeMethod(instance, reader);
            }
        }

        #region 池

        private static Pool<MarkupWriter> _writerPool = new Pool<MarkupWriter>(() =>
        {
            return new MarkupWriter();
        }, (writer, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                writer.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        private static Pool<MarkupReader> _readerPool = new Pool<MarkupReader>(() =>
        {
            return new MarkupReader();
        }, (reader, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                reader.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        #endregion

        #region 得到TypeInfo


        private static readonly Func<Type, TypeMakupInfo> _getTypeInfo;

        static TypeMakupInfo()
        {
            _getTypeInfo = LazyIndexer.Init<Type, TypeMakupInfo>(classType => new TypeMakupInfo(classType));
        }

        public static TypeMakupInfo GetTypeInfo(Type classType)
        {
            return _getTypeInfo(classType);
        }

        #endregion

    }
}

