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
    internal abstract class TypeSerializationInfo
    {
        public DTOClassAttribute ClassAttribute
        {
            get;
            private set;
        }

        /// <summary>
        /// 被序列化的类型
        /// </summary>
        public Type ClassType
        {
            get;
            private set;
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


        #region 初始化

        protected abstract DTOClassAttribute GetClassAttribute(Type classType);

        protected void Initialize()
        {
            if (this.ClassAttribute != null) return; //已初始化
            this.ClassAttribute = GetClassAttribute(this.ClassType);
            if (SerializationMethodHelper.IsPrimitive(this.ClassType) || this.ClassType == typeof(DTObject))
            {
                //this.DTOSchemaCode = string.Empty;
            }
            else {
                _memberInfos = BuildMembers();
                SerializeMethod = CreateSerializeMethod();
                DeserializeMethod = CreateDeserializeMethod();
               // this.DTOSchemaCode = "{}";
            }
        }

        protected abstract DTOMemberAttribute GetMemberAttribute(MemberInfo member);

        private List<MemberSerializationInfo> BuildMembers()
        {
            var memberInfos = new List<MemberSerializationInfo>();

            if (this.IsCollection) //类型是集合，因此类型本身也要加入序列化
                memberInfos.Add(MemberSerializationInfo.Create(this.ClassType));

            const BindingFlags memberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var members = this.ClassType.GetPropertyAndFields(memberFlags);
            foreach (MemberInfo member in members)
            {
                if (member is FieldInfo)
                {
                    DTOMemberAttribute attr = GetMemberAttribute(member);
                    if (attr == null) continue;

                    memberInfos.Add(MemberSerializationInfo.Create(member, attr));
                }
                else
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        DTOMemberAttribute attr = GetMemberAttribute(member);
                        if (attr == null) continue;

                        if (!(property.CanRead || property.CanWrite)) //因为有可能对象值需要写入或者只需要读取，因此只要有其中一种就可以
                            throw new SerializationException(string.Format("{0}.{1} 必须定义读取或写入的方法", this.ClassType.Name, property.Name));
                        memberInfos.Add(MemberSerializationInfo.Create(member, attr));
                    }
                }
            }

            OnBuildMembers(memberInfos);

            return memberInfos;
        }

        /// <summary>
        /// 当构建成员信息完成时触发
        /// </summary>
        /// <param name="members"></param>
        protected abstract void OnBuildMembers(List<MemberSerializationInfo> members);

        #endregion

        #region 得到TypeInfo


        protected TypeSerializationInfo(Type classType)
        {
            if (classType == null) throw new ArgumentNullException("type");
            if (classType.IsInterface) throw new ArgumentException("类型不能为接口", "type");
            if (classType.IsAbstract) throw new ArgumentException("类型不能为抽象的", "type");
            this.ClassType = classType;
        }

        #endregion

        #region 序列化方法


        public SerializeMethod SerializeMethod
        {
            get;
            private set;
        }

        private SerializeMethod CreateSerializeMethod()
        {
            return DTOSerializeMethodGenerator.GenerateMethod(this);
        }

        #endregion

        #region 反序列化方法


        public DeserializeMethod DeserializeMethod
        {
            get;
            private set;
        }

        private DeserializeMethod CreateDeserializeMethod()
        {
            return DTODeserializeMethodGenerator.GenerateMethod(this);
        }

        #endregion

        public DTObject Serialize(object instance)
        {
            var dto = DTObject.Create();

            var serializable = instance as IDTOSerializable;
            if(serializable != null)
            {
                serializable.Serialize(dto, string.Empty); //string.Empty意味着 序列化的内容会完全替换dto
            }
            else
            {
                Serialize(instance, dto);
            }
            return dto;
        }

        /// <summary>
        /// 将对象<paramref name="instance"/>的信息序列化到<paramref name="dto"/>里
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dto"></param>
        public abstract void Serialize(object instance, DTObject dto);


        public object Deserialize(DTObject dto)
        {
            var instance = this.ClassType.CreateInstance();
            Deserialize(instance, dto);
            return instance;
        }

        /// <summary>
        /// 用<paramref name="dto"/>里的数据，填充<paramref name="instance"/>的属性
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public abstract void Deserialize(object instance, DTObject dto);


    }
}

