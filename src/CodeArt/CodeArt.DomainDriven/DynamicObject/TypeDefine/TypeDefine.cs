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

        public string MetadataSchemaCode
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
            private set;
        }

        /// <summary>
        /// 类型限定名（也就是在根Define中定义的类型的名称）
        /// </summary>
        public string QualifiedName
        {
            get;
            private set;
        }

        public bool CloseMultiTenancy
        {
            get;
            private set;
        }

        /// <summary>
        /// 远程根类
        /// </summary>
        /// <param name="typeName">类型名称，请注意，同一个类型名称的<paramref name="metadataCode"/>代码应该相同</param>
        /// <param name="metadataCode"></param>
        internal TypeDefine(string typeName, string metadataCode, Type domainInterfaceType, Type objectType,bool closeMultiTenancy)
            : this(typeName, metadataCode, DTObject.GetMetadata(FormatMetadataCode(typeName, metadataCode)), domainInterfaceType, objectType, typeName, closeMultiTenancy)
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


        internal TypeDefine(string typeName, TypeMetadata metadata, Type domainInterfaceType, Type objectType, string qualifiedName, bool closeMultiTenancy)
            : this(typeName, metadata.MetadataCode, metadata, domainInterfaceType, objectType, qualifiedName, closeMultiTenancy)
        {

        }

        private TypeDefine(string typeName, string metadataCode, TypeMetadata metadata, Type domainInterfaceType, Type objectType, string qualifiedName,bool closeMultiTenancy)
        {
            this.TypeName = typeName.FirstToUpper();
            this.MetadataCode = metadataCode;
            this.MetadataSchemaCode = DTObject.Create(metadataCode).GetSchemaCode(false, false);
            this.Metadata = metadata;
            this.DomainInterfaceType = domainInterfaceType;
            this.ObjectType = objectType;
            this.Constructor = this.ObjectType.ResolveConstructor(typeof(TypeDefine), typeof(bool));
            this.QualifiedName = qualifiedName;
            this.CloseMultiTenancy = closeMultiTenancy;
            //以下代码是新增截获远程对象删除机制所改的，多个子系统有可能有多个远程对象(User)的定义，在实际使用时，需要定位到某一个User上
            //但是在不同的User上的截获事件都需要触发，所以，为了让忽略的对象不破坏内存定义，改写成以下代码
            if(!IsIgnore(this))
            {
                InitMetadataType();
                this.RemoteType = GetRemoteType();
                AddDefineIndex(typeName, this);  //加入索引
                RemoteType.AddDefineIndex(this.RemoteType.FullName, this); //加入索引
            }
            //以下是老代码
            //InitMetadataType();
            //this.RemoteType = GetRemoteType();
            //if (IsIgnore(this))
            //{
            //    LoadFromLocateType();
            //}
            //else
            //{
            //    AddDefineIndex(typeName, this);  //加入索引
            //    RemoteType.AddDefineIndex(this.RemoteType.FullName, this); //加入索引
            //}
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

        /// <summary>
        /// 快照属性
        /// </summary>
        public DomainProperty SnapshotProperty
        {
            get;
            private set;
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
            if (metadataType != null) this.MetadataType = metadataType;
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

        private void LoadFromLocateType()
        {
            var runtimeObjectType = this.MetadataType as RuntimeObjectType;
            if (runtimeObjectType != null)
            {
                var locateDefine = runtimeObjectType.Define;
                FillBy(locateDefine);
            }
        }

        private void FillBy(TypeDefine target)
        {
            this.TypeName = target.TypeName;
            this.MetadataSchemaCode = target.MetadataSchemaCode;
            this.MetadataCode = target.MetadataCode;
            this.Metadata = target.Metadata
;
            this.ObjectType = target.ObjectType;
            this.Constructor = target.Constructor;
            this.MetadataType = target.MetadataType;

            this.DomainInterfaceType = target.DomainInterfaceType;
            this.QualifiedName = target.QualifiedName;
            this.RemoteType = target.RemoteType;
            this.Properties = target.Properties;
        }



        /// <summary>
        /// 填充元数据类型的信息
        /// </summary>
        /// <param name="metadataType"></param>
        private void FillMetadataType(RuntimeObjectType metadataType)
        {
            metadataType.AddInterface(this.DomainInterfaceType);
            metadataType.AddAttribute(new ObjectRepositoryAttribute(typeof(IDynamicRepository)) { CloseMultiTenancy = this.CloseMultiTenancy });
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


            this.SnapshotProperty = GetSnapshotProperty(objectType);

            properties.Add(this.SnapshotProperty);

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
            object defaultValue = entry.IsString ? string.Empty : DataUtil.GetDefaultValue(propertyType);
            AttachAttributes(propertyInfo, entry);
            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return defaultValue; }, null, propertyType);
        }

        private void AttachAttributes(RuntimePropertyInfo propertyInfo, ValueEntry entry)
        {
            if (entry.IsString)
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
            }
        }


        private DomainProperty GetSnapshotProperty(RuntimeType objectType)
        {
            var propertyName = "Snapshot";
            var propertyType = typeof(bool);
            var propertyInfo = objectType.AddProperty(propertyName, propertyType);
            propertyInfo.AddAttribute(new PropertyRepositoryAttribute());
            object defaultValue = false;
            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return defaultValue; }, null, propertyType);
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

            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return define.EmptyInstance; }, null, propertyType);
        }

        private TypeDefine GetObjectDefine(RuntimeType objectType, ObjectEntry entry)
        {
            var typeName = GetObjectTypeName(entry.TypeName);

            var metadataType = GetMetadataType(typeName);
            if (metadataType != null) return metadataType.Define;

            var idEntry = entry.GetMemberByName(EntityObject.IdPropertyName);
            if (idEntry == null)
            {
               return new ValueObjectDefine(typeName, entry.Metadata, this.QualifiedName,this.CloseMultiTenancy);
            }
            else
            {
                //引用对象
                return new EntityObjectDefine(typeName, entry.Metadata, this.QualifiedName, this.CloseMultiTenancy);
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

            var valueEntry = itemEntry as ValueEntry;
            if(valueEntry != null)
                AttachAttributes(propertyInfo, valueEntry);

            return DomainProperty.Register(propertyName, propertyType, objectType, (o, p) => { return propertyType.CreateInstance(); }, null, elementType);
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

        /// <summary>
        /// 是否忽略类型的定义（因为多个子系统都有同样的定义，那么只使用其中一个）
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="define"></param>
        /// <returns></returns>
        private static bool IsIgnore(TypeDefine define)
        {
            return IsIgnore(define.GetType());

        }

        internal static bool IsIgnore(Type defineType)
        {
            if (_locates.TryGetValue(defineType.Name, out var locateDefineType))
            {
                //如果类型名称在定位中存在，那么判断define是否满足定位的要求
                if (defineType != locateDefineType) return true;
            }
            return false;
        }

        /// <summary>
        /// Type:动态类型定义的类型
        /// </summary>
        private static ConcurrentDictionary<string, Type> _locates = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// 由于多个子系统可能都引用了同一个远程对象，但是这些子系统内部定义的远程对象的架构代码不同，
        /// 该方法可以用于在系统集成中，多个系统定位于同一个远程对象定义，例如：
        /// <para>
        /// 投票子系统、会议系统都在内部定义了User的远程类型，我们可以通过Locate方法，在会议服务中，将User的远程类型定位统一为一个定义
        /// </para>
        /// </summary>
        /// <param name="define"></param>
        public static void Locate(string typeName, Type defineType)
        {
            if (_locates.TryGetValue(typeName, out var exist))
            {
                throw new DomainDrivenException(string.Format(Strings.DynamicTypeRepeated, typeName));
            }

            _locates.TryAdd(typeName, defineType);
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
        public static TypeDefine GetDefine<RT>()
            where RT : TypeDefine
        {
            var define = SafeAccessAttribute.CreateSingleton<RT>();
            return GetDefine(define.TypeName); //为了防止重定向了类型定义，所以要再获取一遍
        }

        public static TypeDefine GetDefine(Type defineType)
        {
            var define = SafeAccessAttribute.CreateSingleton(defineType) as TypeDefine;
            return GetDefine(define.TypeName); //为了防止重定向了类型定义，所以要再获取一遍
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
