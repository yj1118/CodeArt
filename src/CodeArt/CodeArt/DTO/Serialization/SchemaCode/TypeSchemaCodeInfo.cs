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
    /// 基于架构代码的序列化机制
    /// </summary>
    [DebuggerDisplay("{ClassType.Name}")]
    internal class TypeSchemaCodeInfo : TypeSerializationInfo
    {
        private SchemaCodes _schemaCodes;


        public TypeSchemaCodeInfo(Type classType, string schemaCode)
            : base(classType)
        {
            _schemaCodes = new SchemaCodes(classType, schemaCode);
            this.Initialize();
        }


        protected override DTOClassAttribute GetClassAttribute(Type classType)
        {
            return DTOClassAttribute.Default;
        }


        protected override DTOMemberAttribute GetMemberAttribute(MemberInfo member)
        {
            if (_schemaCodes.CanMarkup(member.Name))
                return new DTOMemberAttribute(member.Name, DTOMemberType.General);
            return null;
        }

        protected override void OnBuildMembers(List<MemberSerializationInfo> members)
        {
            _schemaCodes.Sort(members);
        }

        public override void Serialize(object instance, DTObject dto)
        {
            using (var temp = _writerPool.Borrow())
            {
                var writer = temp.Item;
                writer.Initialize(dto, _schemaCodes);
                SerializeMethod(instance, writer);
            }
        }

        public override void Deserialize(object instance, DTObject dto)
        {
            using (var temp = _readerPool.Borrow())
            {
                var reader = temp.Item;
                reader.Initialize(dto, _schemaCodes);
                DeserializeMethod(instance, reader);
            }
        }

        #region 池

        private static Pool<SchemaCodeWriter> _writerPool = new Pool<SchemaCodeWriter>(() =>
        {
            return new SchemaCodeWriter();
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

        private static Pool<SchemaCodeReader> _readerPool = new Pool<SchemaCodeReader>(() =>
        {
            return new SchemaCodeReader();
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


        private static readonly Func<Type, Func<string, TypeSchemaCodeInfo>> _getTypeInfo;

        static TypeSchemaCodeInfo()
        {
            _getTypeInfo = LazyIndexer.Init<Type, Func<string, TypeSchemaCodeInfo>>((classType) =>
            {
                return LazyIndexer.Init<string, TypeSchemaCodeInfo>((schemaCode) =>
                {
                    return new TypeSchemaCodeInfo(classType, schemaCode);
                });
            });
        }

        public static TypeSchemaCodeInfo GetTypeInfo(Type classType, string schemaCode)
        {
            return _getTypeInfo(classType)(schemaCode);
        }

        #endregion
    }
}

