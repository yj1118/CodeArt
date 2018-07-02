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
    /// 为指定的类型存储序列化信息
    /// </summary>
    [DebuggerDisplay("{_classType.Name}")]
    internal class TypeSerializationInfo
    {
        public DTOClassAttribute ClassAttribute
        {
            get;
            private set;
        }

        private Type _classType = null;
        /// <summary>
        /// 被序列化的类型
        /// </summary>
        public Type ClassType
        {
            get { return _classType; }
        }

        /// <summary>
        /// 被序列化的类型为值类型
        /// </summary>
        public bool IsValueType
        {
            get
            {
                return this.ClassType.IsValueType;
            }
        }

        private List<MemberSerializationInfo> _memberInfos;
        public IEnumerable<MemberSerializationInfo> MemberInfos
        {
            get
            {
                return _memberInfos;
            }
        }


        /// <summary>
        /// 是否为集合类，例如 array、collection、dictionary等
        /// </summary>
        public bool IsCollection
        {
            get { return this.ClassType.IsCollection(); }
        }

        //public string DTOSchemaCode
        //{
        //    get;
        //    private set;
        //}


        #region 初始化

        private void Init()
        {
            if (this.ClassAttribute != null) return; //已初始化
            this.ClassAttribute = DTOClassAttribute.GetAttribute(_classType);
            if (SerializationMethodHelper.IsPrimitive(this.ClassType) || this.ClassType == typeof(DTObject))
            {
                //this.DTOSchemaCode = string.Empty;
            }
            else {
                _memberInfos = BuildMembers();
                _serializeMethod = CreateSerializeMethod();
                _deserializeMethod = CreateDeserializeMethod();
               // this.DTOSchemaCode = "{}";
            }
        }

        private List<MemberSerializationInfo> BuildMembers()
        {
            var memberInfos = new List<MemberSerializationInfo>();

            if (this.IsCollection) //类型是集合，因此类型本身也要加入序列化
                memberInfos.Add(MemberSerializationInfo.Create(this.ClassType));

            const BindingFlags memberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (MemberInfo member in _classType.GetMembers(memberFlags))
            {
                DTOMemberAttribute attr = DTOMemberAttribute.GetAttribute(member);
                if (attr == null) continue;

                if (member is FieldInfo) memberInfos.Add(MemberSerializationInfo.Create(member, attr));
                else
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        if (!(property.CanRead && property.CanWrite))
                            throw new SerializationException(string.Format("{0}.{1} 必须定义读取和写入的方法", _classType.Name, property.Name));
                        memberInfos.Add(MemberSerializationInfo.Create(member, attr));
                    }
                }
            }

            memberInfos.Sort(MemberSerializationInfoComparator.Instance);

            return memberInfos;
        }

        #endregion

        #region 得到TypeInfo

        private static readonly Func<Type, TypeSerializationInfo> _getTypeInfo;

        static TypeSerializationInfo()
        {
            _getTypeInfo = LazyIndexer.Init<Type, TypeSerializationInfo>(classType => new TypeSerializationInfo(classType));
        }

        public static TypeSerializationInfo GetTypeInfo(Type classType)
        {
            return _getTypeInfo(classType);
        }

        private TypeSerializationInfo(Type classType)
        {
            if (classType == null) throw new ArgumentNullException("type");
            if (classType.IsInterface) throw new ArgumentException("类型不能为接口", "type");
            if (classType.IsAbstract) throw new ArgumentException("类型不能为抽象的", "type");
            _classType = classType;
            Init();
        }

        #endregion


        //private string CreateDTOSchemaCode()
        //{
        //    if (SerializationMethodHelper.IsPrimitive(this.ClassType)) return string.Empty;
        //    if (this.ClassAttribute.Mode == DTOSerializableMode.General)
        //    {
        //        StringBuilder code = new StringBuilder();
        //        foreach (var member in this.MemberInfos)
        //        {
        //            code.AppendFormat("{0},", member.GetDTOSchemaCode());
        //        }
        //        if (code.Length > 0) code.Length--;
        //        code.Insert(0, "{");
        //        code.Append("}");
        //        return code.ToString();
        //    }
        //    else
        //    {
        //        StringBuilder code = new StringBuilder();
        //        foreach (var member in this.MemberInfos)
        //        {
        //            if (member.MemberAttribute.Type == DTOMemberType.ReturnValue)
        //            {
        //                code.AppendFormat("{0},", member.GetDTOSchemaCode());
        //            }
        //        }
        //        if (code.Length > 0) code.Length--;
        //        code.Insert(0, "{");
        //        code.Append("}");
        //        return code.ToString();

        //    }
        //}



        #region 序列化方法


        private SerializeMethod _serializeMethod;

        private SerializeMethod CreateSerializeMethod()
        {
            return DTOSerializeMethodGenerator.GenerateMethod(this);
        }

        #endregion

        #region 反序列化方法


        private DeserializeMethod _deserializeMethod;

        private DeserializeMethod CreateDeserializeMethod()
        {
            return DTODeserializeMethodGenerator.GenerateMethod(this);
        }

        #endregion

        public DTObject Serialize(object instance)
        {
            using (var temp = _writerPool.Borrow())
            {
                var writer = temp.Item;
                writer.Initialize();
                _serializeMethod(instance, writer);
                return writer.GetDTO();
            }
        }


        public object Deserialize(DTObject dto)
        {
            using (var temp = _readerPool.Borrow())
            {
                var reader = temp.Item;
                reader.Initialize(dto);
                return _deserializeMethod(reader);
            }
        }



        #region 池

        private static Pool<DTOWriter> _writerPool = new Pool<DTOWriter>(() =>
        {
            return new DTOWriter();
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

        private static Pool<DTOReader> _readerPool = new Pool<DTOReader>(() =>
        {
            return new DTOReader();
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


    }
}

