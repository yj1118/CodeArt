using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml
{
    public class DependencyAction : IDependencyAction
    {
        private DependencyAction() { }

        /// <summary>
        /// 行为名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 拥有该行为的类型
        /// </summary>
        public Type OwnerType { get; private set; }

        public bool AllowClientAccess { get; private set; }

        /// <summary>
        /// 默认元数据
        /// </summary>
        public ActionMetadata DefaultMetadata { get; private set; }

        /// <summary>
        /// 执行过程
        /// </summary>
        public ActionProcedure Procedure
        {
            get
            {
                return this.DefaultMetadata.Procedure;
            }
        }


        public PreCallActionCallback PreCallActionCallback
        {
            get
            {
                return this.DefaultMetadata.PreCallActionCallback;
            }
        }

        /// <summary>
        /// 是否注册了preCallAction事件
        /// </summary>
        public bool IsRegisteredPreCall
        {
            get
            {
                return this.DefaultMetadata.IsRegisteredPreCall;
            }
        }

        internal void OnPreCall(object sender, DependencyActionPreCallEventArgs e)
        {
            this.DefaultMetadata.OnPreCall(sender, e);
        }

        public event DependencyActionPreCallEventHandler PreCall
        {
            add
            {
                this.DefaultMetadata.PreCall += value;
            }
            remove
            {
                this.DefaultMetadata.PreCall -= value;
            }
        }


        public CalledActionCallback CalledActionCallback
        {
            get
            {
                return this.DefaultMetadata.CalledActionCallback;
            }
        }

        /// <summary>
        /// 是否注册了preSetValue事件
        /// </summary>
        public bool IsRegisteredCalled
        {
            get
            {
                return this.DefaultMetadata.IsRegisteredCalled;
            }
        }

        internal void OnCalled(object sender, DependencyActionCalledEventArgs e)
        {
            this.DefaultMetadata.OnCalled(sender, e);
        }

        public event DependencyActionCalledEventHandler Called
        {
            add
            {
                this.DefaultMetadata.Called += value;
            }
            remove
            {
                this.DefaultMetadata.Called -= value;
            }
        }


        public Guid Id { get; private set; }

        public override bool Equals(object obj)
        {
            var target = obj as DependencyAction;
            if (target == null) return false;
            return this.Id == target.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(DependencyAction action0, DependencyAction action1)
        {
            return object.Equals(action0, action1);
        }

        public static bool operator !=(DependencyAction action0, DependencyAction action1)
        {
            return !object.Equals(action0, action1);
        }

        #region 静态方法

        public static DependencyAction Register(string name, Type ownerType,bool allowClientAccess, ActionMetadata defaultMetadata)
        {
            var action = new DependencyAction()
            {
                Id = Guid.NewGuid(),
                Name = name,
                OwnerType = ownerType,
                AllowClientAccess = allowClientAccess,
                DefaultMetadata = defaultMetadata
            };
            lock (_actions)
                _actions.Add(action);
            return action;
        }


        public static DependencyAction Register<OT>(string name, bool allowClientAccess, ActionMetadata defaultMetadata)
        {
            return Register(name, typeof(OT), allowClientAccess, defaultMetadata);
        }

        public static DependencyAction GetAction(Type dependencyObjectType, string actionName)
        {
            return _getActionByName(dependencyObjectType)(actionName);
        }

        private static Func<Type, Func<string, DependencyAction>> _getActionByName = LazyIndexer.Init<Type, Func<string, DependencyAction>>((dependencyObjectType) =>
        {
            return LazyIndexer.Init<string, DependencyAction>((actionName) =>
            {
                var da = FindAction(dependencyObjectType, actionName);
                if (da != null) return da;

                //如果没有找到，实例化一次组件,防止组件没有运行，导致没有注册依赖属性
                var obj = Activator.CreateInstance(dependencyObjectType);
                //再找一次
                da = FindAction(dependencyObjectType, actionName);
                if (da != null) return da;

                return null; //不抛出异常
                //throw new XamlException("在类型" + dependencyObjectType.FullName + "和其继承链上没有找到依赖方法" + actionName + "的定义");

            });
        });

        private static DependencyAction FindAction(Type dependencyObjectType, string actionName)
        {
            foreach (var a in _actions)
            {
                if (dependencyObjectType.IsImplementOrEquals(a.OwnerType) && a.Name.Equals(actionName, StringComparison.Ordinal)) return a;
            }
            return null;
        }


        private static List<DependencyAction> _actions = new List<DependencyAction>();

        #endregion
    }
}