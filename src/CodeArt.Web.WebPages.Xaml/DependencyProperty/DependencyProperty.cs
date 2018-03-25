using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Web.WebPages.Xaml.Markup;


namespace CodeArt.Web.WebPages.Xaml
{
    [DebuggerDisplay("Type = DependencyProperty, Name = {Name}")]
    public class DependencyProperty : IDependencyProperty
    {
        private DependencyProperty(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 属性的类型
        /// </summary>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// 拥有该属性的类型
        /// </summary>
        public Type OwnerType { get; private set; }


        /// <summary>
        /// 默认元数据
        /// </summary>
        public PropertyMetadata DefaultMetadata { get; private set; }

        public Func<object> GetDefaultValue
        {
            get
            {
                return this.DefaultMetadata.GetDefaultValue;
            }
        }

        public PropertyChangedCallback ChangedCallBack
        {
            get
            {
                return this.DefaultMetadata.ChangedCallBack;
            }
        }

        public PreSetValueCallback PreSetValueCallback
        {
            get
            {
                return this.DefaultMetadata.PreSetValueCallback;
            }
        }

        public GotValueCallback GotValueCallback
        {
            get
            {
                return this.DefaultMetadata.GotValueCallback;
            }
        }

        /// <summary>
        /// 是否注册了changed事件
        /// </summary>
        public bool IsRegisteredChanged
        {
            get
            {
                return this.DefaultMetadata.IsRegisteredChanged;
            }
        }

        internal void OnChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.DefaultMetadata.OnChanged(sender, e);
        }

        public event DependencyPropertyChangedEventHandler Changed
        {
            add
            {
                this.DefaultMetadata.Changed += value;
            }
            remove
            {
                this.DefaultMetadata.Changed -= value;
            }
        }

        /// <summary>
        /// 是否注册了preSetValue事件
        /// </summary>
        public bool IsRegisteredPreSet
        {
            get
            {
                return this.DefaultMetadata.IsRegisteredPreSet;
            }
        }

        internal void OnPreSet(object sender, DependencyPropertyPreSetEventArgs e)
        {
            this.DefaultMetadata.OnPreSet(sender, e);
        }

        public event DependencyPropertyPreSetEventHandler PreSet
        {
            add
            {
                this.DefaultMetadata.PreSet += value;
            }
            remove
            {
                this.DefaultMetadata.PreSet -= value;
            }
        }

        /// <summary>
        /// 是否注册了preSetValue事件
        /// </summary>
        public bool IsRegisteredGot
        {
            get
            {
                return this.DefaultMetadata.IsRegisteredGot;
            }
        }

        internal void OnGot(object sender, DependencyPropertyGotEventArgs e)
        {
            this.DefaultMetadata.OnGot(sender, e);
        }

        public event DependencyPropertyGotEventHandler Got
        {
            add
            {
                this.DefaultMetadata.Got += value;
            }
            remove
            {
                this.DefaultMetadata.Got -= value;
            }
        }

        /// <summary>
        /// 每一个属性的编号，全局唯一
        /// </summary>
        public Guid Id { get; private set; }

        public override bool Equals(object obj)
        {
            var target = obj as DependencyProperty;
            if (target == null) return false;
            return this.Id == target.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(DependencyProperty property0, DependencyProperty property1)
        {
            return object.Equals(property0, property1);
        }

        public static bool operator !=(DependencyProperty property0, DependencyProperty property1)
        {
            return !object.Equals(property0, property1);
        }

        #region 静态方法

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType,PropertyMetadata defaultMetadata)
        {
            var property = new DependencyProperty(Guid.NewGuid())
            {
                Name = name,
                PropertyType = propertyType,
                OwnerType = ownerType,
                DefaultMetadata = defaultMetadata
            };
            lock (_properties)
                _properties.Add(property);
            return property;
        }

     

        public static DependencyProperty Register<PT, OT>(string name, PropertyMetadata defaultMetadata)
        {
            return Register(name, typeof(PT), typeof(OT), defaultMetadata);
        }

        public static DependencyProperty Register<PT, OT>(string name, Func<object> getDefaultValue)
        {
            return Register(name, typeof(PT), typeof(OT), new PropertyMetadata(getDefaultValue));
        }

        public static DependencyProperty GetProperty(Type dependencyObjectType, string propertyName)
        {
            return _getPropertyByName(dependencyObjectType)(propertyName);
        }

        internal static DependencyProperty GetProperty(Type dependencyObjectType, Guid propertyId)
        {
            return _getPropertyById(dependencyObjectType)(propertyId);
        }

        #endregion


        private static Func<Type, Func<string, DependencyProperty>> _getPropertyByName = LazyIndexer.Init<Type, Func<string, DependencyProperty>>((dType) =>
        {
            return LazyIndexer.Init<string, DependencyProperty>((propertyName) =>
            {
                var dp = FindProperty(dType, propertyName);
                if (dp != null) return dp;
                if (!IsRegistered(dType))
                {
                    //如果没有找到，实例化一次组件,防止组件没有运行，导致没有注册依赖属性
                    var obj = Activator.CreateInstance(dType);
                    //再找一次
                    dp = FindProperty(dType, propertyName);
                    if (dp != null) return dp;
                }
                return null;
                //throw new XamlException("在类型" + dependencyObjectType.FullName + "和其继承链上没有找到依赖属性" + propertyName + "的定义");
            });
        });

        private static DependencyProperty FindProperty(Type dependencyObjectType, string propertyName)
        {
            foreach (var p in _properties)
            {
                if (dependencyObjectType.IsImplementOrEquals(p.OwnerType) && p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) return p;
            }
            return null;
        }

        private static Func<Type, Func<Guid, DependencyProperty>> _getPropertyById = LazyIndexer.Init<Type, Func<Guid, DependencyProperty>>((dType) =>
        {
            return LazyIndexer.Init<Guid, DependencyProperty>((propertyId) =>
            {
                var dp = FindProperty(propertyId);
                if (dp != null) return dp;

                if (!IsRegistered(dType))
                {
                    //如果没有找到，实例化一次组件,防止组件没有运行，导致没有注册依赖属性
                    var obj = Activator.CreateInstance(dType);
                    //再找一次
                    dp = FindProperty(propertyId);
                    if (dp != null) return dp;
                }

                throw new XamlException("在没有找到依赖项属性" + propertyId + "的定义");
            });
        });

        private static DependencyProperty FindProperty(Guid propertyId)
        {
            foreach (var p in _properties)
            {
                if (p.Id == propertyId) return p;
            }
            return null;
        }

        private static List<DependencyProperty> _properties = new List<DependencyProperty>();

        private static ConcurrentDictionary<Type, bool> _registered = new ConcurrentDictionary<Type, bool>();
        /// <summary>
        /// 标记类型已经注册了依赖属性，也就是至少构造过一次该对象
        /// </summary>
        /// <param name="dType"></param>
        internal static void MarkRegistered(Type dType)
        {
            _registered.TryAdd(dType, true);
        }

        private static bool IsRegistered(Type dType)
        {
            return _registered.ContainsKey(dType);
        }

    }
}