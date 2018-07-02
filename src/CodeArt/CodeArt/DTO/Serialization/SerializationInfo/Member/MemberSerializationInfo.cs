using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DTO
{
    /// <summary>
    /// 为指定类型上的属性存储序列化信息
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal class MemberSerializationInfo
    {
        #region 静态构造

        private static Type GetTargetType(Type classType, MemberInfo memberInfo)
        {
            if (classType != null) return classType;
            PropertyInfo p = memberInfo as PropertyInfo;
            if (p != null) return p.PropertyType;
            return (memberInfo as FieldInfo).FieldType;
        }

        private static MemberSerializationInfo CreateByCollection(Type targetType, MemberInfo memberInfo, DTOMemberAttribute memberAttribute)
        {
            Type _collectionInterfaceType = null;
            Type _dictionaryInterfaceType = null;

            if (targetType.FindCollectionInterface(ref _collectionInterfaceType,
                          ref _dictionaryInterfaceType))
            {
                if (_dictionaryInterfaceType != null) //IDictionary{Key,Value}
                    throw new DTOException("暂时不支持键值对的dto序列化操作");
                //return memberInfo == null ? new DictionarySerializationInfo(targetType) : new DictionarySerializationInfo(memberInfo, memberAttribute);
                else //ICollection{T}
                    return memberInfo == null ? new CollectionSerializationInfo(targetType) : new CollectionSerializationInfo(memberInfo, memberAttribute);
            }
            return null;
        }

        public static MemberSerializationInfo Create(MemberInfo memberInfo, DTOMemberAttribute memberAttribute)
        {
            Type t = GetTargetType(null, memberInfo);
            //数组
            if (t.IsArray) return new ArraySerializationInfo(memberInfo, memberAttribute);
            //ICollection或IDictionary
            MemberSerializationInfo info = CreateByCollection(t, memberInfo, memberAttribute);
            if (info != null) return info;
            //普通类型
            return new MemberSerializationInfo(memberInfo, memberAttribute);
        }

       
        public static MemberSerializationInfo Create(Type classType)
        {
            Type t = classType;
            //数组
            if (t.IsArray) return new ArraySerializationInfo(classType);
            //ICollection或IDictionary
            MemberSerializationInfo info = CreateByCollection(classType, null, null);
            if (info != null) return info;
            //普通类型
            return new MemberSerializationInfo(classType);
        }

        #endregion

        private MemberInfo _memberInfo;
        private Type _classType;

        private DTOMemberAttribute _memberAttribute;
        public DTOMemberAttribute MemberAttribute
        {
            get { return _memberAttribute; }
        }

        /// <summary>
        /// 属性的名称
        /// </summary>
        public string Name
        {
            get
            {
                return _memberInfo.Name;
            }
        }

        public Type OwnerType
        {
            get
            {
                if (_classType != null) return _classType;
                return _memberInfo.DeclaringType;
            }
        }

        public bool CanWrite
        {
            get
            {
                if (this.IsFieldInfo) return true;
                return this.PropertyInfo.CanWrite;
            }
        }

        public bool CanRead
        {
            get
            {
                if (this.IsFieldInfo) return true;
                return this.PropertyInfo.CanRead;
            }
        }

        public bool IsAbstract
        {
            get
            {
                return _memberInfo.GetFieldOrPropertyType().IsAbstract;
            }
        }



        #region 序列化的目标

        public Type TargetType
        {
            get
            {
                return GetTargetType(_classType, _memberInfo);
            }
        }

        /// <summary>
        /// 序列化的目标是一个属性
        /// </summary>
        public bool IsProperty
        {
            get
            {
                return _memberInfo is PropertyInfo;
            }
        }

        public PropertyInfo PropertyInfo
        {
            get
            {
                return _memberInfo as PropertyInfo;
            }
        }

        public bool IsFieldInfo
        {
            get
            {
                return _memberInfo is FieldInfo;
            }
        }

        public FieldInfo FieldInfo
        {
            get
            {
                return _memberInfo as FieldInfo;
            }
        }

        public bool IsClassInfo
        {
            get
            {
                return _classType != null;
            }
        }

        #endregion


        public MemberSerializationInfo(Type classType)
        {
            _classType = classType;
        }

        public MemberSerializationInfo(MemberInfo memberInfo, DTOMemberAttribute memberAttribute)
        {
            _memberInfo = memberInfo;
            _memberAttribute = memberAttribute;
        }


        /// <summary>
        /// 生成序列化代码
        /// </summary>
        /// <param name="g"></param>
        public virtual void GenerateSerializeIL(MethodGenerator g)
        {
            //serializer.serialize(v); 或 //writer.Writer(v);
            SerializationMethodHelper.Write(g, this.DTOMemberName, this.TargetType, (argType) =>
             {
                 LoadMemberValue(g);
                 //TargetType是成员（也就是属性或者字段）的类型
                 //argType是方法需要接受到的类型，如果两者类型不匹配，就需要转换
                 if (this.TargetType.IsStruct() && !argType.IsStruct())
                 {
                     g.Box();
                 }
             });
        }

        /// <summary>
        /// 加载成员的值到堆栈上
        /// </summary>
        protected void LoadMemberValue(MethodGenerator g)
        {
            LoadOwner(g);

            if (this.IsClassInfo) return;
            if (this.IsFieldInfo)
                g.LoadField(this.FieldInfo);
            else
                g.LoadProperty(this.PropertyInfo);
        }



        /// <summary>
        /// 生成反序列化代码
        /// </summary>
        /// <param name="g"></param>
        public virtual void GenerateDeserializeIL(MethodGenerator g)
        {
            SetMember(g, () =>
            {
                SerializationMethodHelper.Read(g, this.DTOMemberName, this.TargetType);
            });
        }

        public void SetMember(MethodGenerator g, Action loadValue)
        {
            if (this.IsClassInfo)
            {
                g.AssignVariable(SerializationArgs.InstanceName, loadValue);
            }
            else
            {
                LoadOwner(g);

                if (this.IsFieldInfo)
                {
                    g.Assign(this.FieldInfo, loadValue);
                }
                else
                {
                    g.Assign(this.PropertyInfo, loadValue);
                }
            }
        }

        private void LoadOwner(MethodGenerator g)
        {
            if (this.OwnerType.IsValueType)
                g.LoadVariable(SerializationArgs.InstanceName, LoadOptions.ValueAsAddress);
            else
                g.LoadVariable(SerializationArgs.InstanceName);
        }


        public string DTOMemberName
        {
            get
            {
                if (this.MemberAttribute != null && !string.IsNullOrEmpty(this.MemberAttribute.Name)) return this.MemberAttribute.Name;
                if (!string.IsNullOrEmpty(_memberInfo.Name)) return _memberInfo.Name.FirstToUpper();
                return string.Empty;
            }
        }


        public virtual string GetDTOSchemaCode()
        {
            return this.DTOMemberName;
        }


    }
}
