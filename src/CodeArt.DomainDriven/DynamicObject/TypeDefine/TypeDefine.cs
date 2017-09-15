using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Concurrent;
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
    /// <summary>
    /// 动态领域类型的定义
    /// </summary>
    [DebuggerDisplay("MetadataCode = {MetadataCode}")]
    public abstract class TypeDefine
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// 类型的元数据码
        /// </summary>
        public string MetadataCode
        {
            get;
            private set;
        }

        internal TypeMetadata Metadata
        {
            get;
            private set;
        }


        /// <summary>
        /// 类型定义对应的实例类型（也就是真正需要创建的、实际存在内存中的对象类型）
        /// </summary>
        internal Type ObjectType
        {
            get;
            private set;
        }

        internal ConstructorInfo Constructor
        {
            get;
            private set;
        }


        /// <summary>
        /// 元数据类型，记录了对象拥有的领域属性等数据
        /// </summary>
        internal Type MetadataType
        {
            get;
            private set;
        }

        private object _emptyInstance;

        /// <summary>
        /// 获取该类型的领域对象的空值
        /// </summary>
        public object EmptyInstance
        {
            get
            {
                if(_emptyInstance == null)
                {
                    _emptyInstance = GetEmptyInstance();
                }
                return _emptyInstance;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal protected abstract object GetEmptyInstance();


        internal Type DomainInterfaceType
        {
            get;
        }

        /// <summary>
        /// 类型限定名（也就是在根Define中定义的类型的名称）
        /// </summary>
        public string QualifiedName
        {
            get;
            private set;
        }


        /// <summary>
        /// 远程根类
        /// </summary>
        /// <param name="typeName">类型名称，请注意，同一个类型名称的<paramref name="metadataCode"/>代码应该相同</param>
        /// <param name="metadataCode"></param>
        internal TypeDefine(string typeName, string metadataCode, Type domainInterfaceType, Type objectType)
            : this(typeName, metadataCode, DTObject.GetMetadata(FormatMetadataCode(typeName, metadataCode)), domainInterfaceType, objectType, typeName)
        {
        }

        /// <summary>
        /// 使用该方法格式化代码后，可以将根类型的名称带入到分析中
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="metadataCode"></param>
        /// <returns></returns>
        private static string FormatMetadataCode(string typeName, string metadataCode)
        {
            if (!metadataCode.StartsWith(typeName, StringComparison.OrdinalIgnoreCase)) return string.Format("{0}:{1}", typeName, metadataCode);
            return metadataCode;
        }


        internal TypeDefine(string typeName, TypeMetadata metadata, Type domainInterfaceType, Type objectType, string qualifiedName)
            : this(typeName, metadata.MetadataCode, metadata, domainInterfaceType, objectType, qualifiedName)
        {

        }

        private TypeDefine(string typeName, string metadataCode, TypeMetadata metadata, Type domainInterfaceType, Type objectType, string qualifiedName)
        {
            this.TypeName = typeName.FirstToUpper();
            this.MetadataCode = metadataCode;
            this.Metadata = metadata;
            this.DomainInterfaceType = domainInterfaceType;
            this.ObjectType = objectType;
            this.Constructor = this.ObjectType.ResolveConstructor(typeof(TypeDefine), typeof(bool));
            this.QualifiedName = qualifiedName;
            InitMetadataType();
            AddDefineIndex(typeName, this);  //加入索引
            this.RemoteType = GetRemoteType();
            RemoteType.AddDefineIndex(this.RemoteType.FullName, this); //加入索引
        }


        public IEnumerable<DomainProperty> Properties
        {
            get;
            private set;
        }

        public DomainProperty GetProperty(string propertyName)
        {
            return this.Properties.FirstOrDefault((p)=>
            {
                return p.Name.EqualsIgnoreCase(propertyName);
            });
        }

        #region 构建类型

        /// <summary>
        /// 构建的类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ns"></param>
        /// <param name="relatedType"></param>
        /// <param name="metadataCode"></param>
        private void InitMetadataType()
        {
            var metadataType = GetMetadataType(this.TypeName);
            if(metadataType != null) this.MetadataType = metadataType;
            else
            {
                metadataType = new RuntimeObjectType(this.TypeName, this);
                //先加入索引
                AddMetadataType(this.TypeName, metadataType); //为了防止由于类型嵌套导致的死循环，我们先把类型加入索引
                //先赋值
                this.MetadataType = metadataType;
                FillMetadataType(metadataType);
                RegisterProperies(metadataType);
            }
        }

        /// <summary>
        /// 填充元数据类型的信息
        /// </summary>
        /// <param name="metadataType"></param>
        private void FillMetadataType(RuntimeObjectType metadataType)
        {
            metadataType.AddInterface(this.DomainInterfaceType);
            metadataType.AddAttribute(new ObjectRepositoryAttribute(typeof(IDynamicRepository)));
        }

        /// <summary>
        /// 设置类型所属的领域属性的信息
        /// </summary>
        /// <param name="belongProperty"></param>
        internal abstract void SetBelongProperty(DomainProperty belongProperty);


        /// <summary>
        /// 注册领域属性
        /// </summary>
        /// <param name="metadataCode"></param>
        private void RegisterProperies(RuntimeType objectType)
        {
            List<DomainProperty> properties = new List<DomainProperty>();
            var metadata = this.Metadata;
            foreach(var entry in metadata.Entries)
            {
                DomainProperty property = null;

                var valueEntry = entry as ValueEntry;
                if (valueEntry != null)
                {
                    property = GetValueProperty(objectType, valueEntry);
                    properties.Add(property);
                    continue;
                }

                var objectEntry = entry as ObjectEntry;
                if (objectEntry != null)
                {
                    property = GetObjectProperty(objectType, objectEntry);
                    properties.Add(property);
                    continue;
                }

                var listEntry = entry as ListEntry;
                if (listEntry != null)
                {
                    property = GetListProperty(objectType, listEntry);
                    properties.Add(property);
                    continue;
                }

            }

            Properties = properties;
        }


        #region 基元值的属性


        private Type GetValueType(ValueEntry entry)
        {
            var type = DataUtil.GetPrimitiveType(entry.TypeName);
            if(type == null)
            {
                if (entry.TypeName == "ascii") return typeof(string);
                throw new DomainDrivenException(string.Format(Strings.UnrecognizedType, entry.TypeName));
            }
            return type;
        }

        private DomainProperty GetValueProperty(RuntimeType objectType, ValueEntry entry)
        {
            var propertyName = entry.Name.FirstToUpper();
            var propertyType = GetValueType(entry);
            var propertyInfo = objectType.AddProperty(propertyName, propertyType);
            propertyInfo.AddAttribute(new PropertyRepositoryAttribute());
            object defaultValue = DataUtil.GetDefaultValue(propertyType);
            //字符串类型处理
            if (entry.TypeName == "string" || entry.TypeName == "ascii")
            {
                if (entry.Descriptions.Count > 0)
                {
                    if (int.TryParse(entry.Descriptions[0], out int max))
                    {
                        propertyInfo.AddAttribute(new StringLengthAttribute(0, max));
                    }

                    if (entry.TypeName == "ascii")
                    {
                        propertyInfo.AddAttribute(new ASCIIStringAttribute());
                    }
                }
                defaultValue = string.Empty;
            }
            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return defaultValue; }, PropertyChangedMode.Compare, propertyType);
        }

        #endregion

        #region 对象的属性

        private DomainProperty GetObjectProperty(RuntimeType objectType, ObjectEntry entry)
        {
            string propertyName = entry.Name.FirstToUpper();
            var define = GetObjectDefine(objectType, entry);
            Type propertyType = define.MetadataType;

            var propertyInfo = objectType.AddProperty(propertyName, propertyType);
            propertyInfo.AddAttribute(new PropertyRepositoryAttribute());

            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return define.EmptyInstance; }, PropertyChangedMode.Definite, propertyType);
        }

        private TypeDefine GetObjectDefine(RuntimeType objectType, ObjectEntry entry)
        {
            var typeName = GetObjectTypeName(entry.TypeName);

            var metadataType = GetMetadataType(typeName);
            if (metadataType != null) return metadataType.Define;

            var idEntry = entry.GetMemberByName(EntityObject.IdPropertyName);
            if (idEntry == null)
            {
               return new ValueObjectDefine(typeName, entry.Metadata, this.QualifiedName);
            }
            else
            {
                //引用对象
                return new EntityObjectDefine(typeName, entry.Metadata, this.QualifiedName);
            }
        }

        private Type GetObjectType(RuntimeType objectType, ObjectEntry entry)
        {
            return GetObjectDefine(objectType, entry).MetadataType;
        }


        private string GetObjectTypeName(string typeName)
        {
            string name = typeName;
            if(typeName.IndexOf('.') > -1)
            {
                var temp = typeName.Split('.');
                StringBuilder sb = new StringBuilder();
                foreach (var t in temp)
                {
                    sb.Append(t.FirstToUpper());
                }
                name = sb.ToString();
            }

            if (name.StartsWith(this.QualifiedName, StringComparison.OrdinalIgnoreCase)) //这种情况是手工指定了限定名
                return name.FirstToUpper();
            return string.Format("{0}{1}", this.QualifiedName, name.FirstToUpper());
        }

        #endregion

        #region 集合属性

        private Type GetType(RuntimeType objectType, TypeEntry entry)
        {
            var valueEntry = entry as ValueEntry;
            if (valueEntry != null)
            {
                return GetValueType(valueEntry);
            }

            var objectEntry = entry as ObjectEntry;
            if (objectEntry != null)
            {
                return GetObjectType(objectType, objectEntry);
            }

            throw new DomainDrivenException(string.Format(Strings.TypeDefineFindTypeError, entry.MetadataCode));
        }


        private DomainProperty GetListProperty(RuntimeType objectType, ListEntry entry)
        {
            var itemEntry = entry.ItemEntry;
            var elementType = GetType(objectType, itemEntry);

            string propertyName = entry.Name.FirstToUpper();

            Type propertyType = GetListType(elementType);

            var propertyInfo = objectType.AddProperty(propertyName, propertyType);
            propertyInfo.AddAttribute(new PropertyRepositoryAttribute());

            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return propertyType.CreateInstance(); }, PropertyChangedMode.Definite, elementType);
        }

        /// <summary>
        /// 获得动态集合的类型
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        internal static Type GetListType(Type elementType)
        {
            Type type = null;
            var runtimeElementType = elementType as RuntimeObjectType;
            if (runtimeElementType != null)
            {
                type = TypeUtil.GetGenericType(typeof(List<>), runtimeElementType.Define.ObjectType);
            }
            else
            {
                type = TypeUtil.GetGenericType(typeof(List<>), elementType);
            }
            return type;
        }


        #endregion


        #endregion

        #region 远程定义

        /// <summary>
        /// 类型的远程定义
        /// </summary>
        public RemoteType RemoteType
        {
            get;
            private set;
        }

        private RemoteType GetRemoteType()
        {
            string typeNamespace = string.Empty;
            string typeName = this.TypeName;

            var attr = AttributeUtil.GetAttribute<RemoteTypeAttribute>(this.GetType());
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.TypeNamespace)) typeNamespace = attr.TypeNamespace;
                if (!string.IsNullOrEmpty(attr.TypeName)) typeName = attr.TypeName;
            }
            //默认情况下命名空间为空，类型名等于本地类型名
            return new RemoteType(typeNamespace, typeName);
        }

        #endregion

        /// <summary>
        /// 以<paramref name="data"/>为数据格式创建前定义的类型的实例
        /// </summary>
        /// <param name="data"></param>
        internal DynamicObject CreateInstance(DTObject data)
        {
            if (data.IsEmpty()) return (DynamicObject)this.GetEmptyInstance();
            DynamicObject obj = null;
            using (var temp = ArgsPool.Borrow2())
            {
                var args = temp.Item;
                args[0] = this;
                args[1] = false;
                obj = (DynamicObject)this.Constructor.CreateInstance(args);
            }
            //加载数据
            obj.Load(data);
            return obj;
        }

        #region TypeDefine索引

        public static TypeDefine GetDefine(string typeName)
        {
            TypeDefine define = null;
            if (_defineIndex.TryGetValue(typeName, out define)) return define;
            throw new DomainDrivenException(string.Format(Strings.NoDyanmicTypeDefine, typeName));
        }

        private static ConcurrentDictionary<string, TypeDefine> _defineIndex = new ConcurrentDictionary<string, TypeDefine>();

        private void AddDefineIndex(string typeName, TypeDefine define)
        {
            TypeDefine exist = null;
            if(_defineIndex.TryGetValue(typeName, out exist))
            {
                if (exist.GetType() == define.GetType()) return;
                throw new DomainDrivenException(string.Format(Strings.DynamicTypeRepeated, typeName));
            }

            _defineIndex.TryAdd(typeName, define);
        }

        #endregion

        #region MetadataType索引

        private static RuntimeObjectType GetMetadataType(string typeName)
        {
            RuntimeObjectType metadataType = null;
            if (_metadataTypes.TryGetValue(typeName, out metadataType)) return metadataType;
            return null;
        }

        private static ConcurrentDictionary<string, RuntimeObjectType> _metadataTypes = new ConcurrentDictionary<string, RuntimeObjectType>();

        private void AddMetadataType(string typeName, RuntimeObjectType metadataType)
        {
            _metadataTypes.TryAdd(typeName, metadataType);
        }

        #endregion


        /// <summary>
        /// 获取类型定义
        /// </summary>
        /// <typeparam name="RT"></typeparam>
        /// <returns></returns>
        public static RT GetDefine<RT>()
            where RT : TypeDefine
        {
            return SafeAccessAttribute.CreateSingleton<RT>();
        }

        /// <summary>
        /// 创建类型定义的实例，该方法主要用于程序初始化时主动初始化类型
        /// </summary>
        /// <param name="define"></param>
        internal static object Initialize(Type typeDefineType)
        {
            return SafeAccessAttribute.CreateSingleton(typeDefineType);
        }

    }
}
