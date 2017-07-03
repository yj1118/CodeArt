using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.DomainDriven
{
    public class DomainAction : IDomainAction
    {
        private DomainAction() { }

        /// <summary>
        /// 行为名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 拥有该行为的类型
        /// </summary>
        public Type OwnerType { get; private set; }

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

        internal void OnPreCall(object sender, DomainActionPreCallEventArgs e)
        {
            this.DefaultMetadata.OnPreCall(sender, e);
        }

        public event DomainActionPreCallEventHandler PreCall
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

        internal void OnCalled(object sender, DomainActionCalledEventArgs e)
        {
            this.DefaultMetadata.OnCalled(sender, e);
        }

        public event DomainActionCalledEventHandler Called
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
            var target = obj as DomainAction;
            if (target == null) return false;
            return this.Id == target.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(DomainAction action0, DomainAction action1)
        {
            return object.Equals(action0, action1);
        }

        public static bool operator !=(DomainAction action0, DomainAction action1)
        {
            return !object.Equals(action0, action1);
        }

        #region 静态方法

        public static DomainAction Register(string name, Type ownerType, ActionMetadata defaultMetadata)
        {
            return new DomainAction()
            {
                Id = Guid.NewGuid(),
                Name = name,
                OwnerType = ownerType,
                DefaultMetadata = defaultMetadata
            };
        }


        public static DomainAction Register<OT>(string name, ActionMetadata defaultMetadata)
        {
            return Register(name, typeof(OT), defaultMetadata);
        }

        #endregion
    }
}