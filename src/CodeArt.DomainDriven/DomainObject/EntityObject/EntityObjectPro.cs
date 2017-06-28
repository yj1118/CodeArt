using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 实体对象的富版本，可以依赖于内聚根进行单独的对象存储
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TIdentity"></typeparam>
    public abstract class EntityObjectPro<TRoot, TObject, TIdentity> : Repositoryable<TObject, TIdentity>, IEntityObjectPro<TRoot>, IEntityObjectPro
        where TObject : EntityObjectPro<TRoot, TObject, TIdentity>
        where TIdentity : struct
        where TRoot : IAggregateRoot
    {
        /// <summary>
        /// 该属性提供ORM使用
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static Type RootType = typeof(TRoot);


        public EntityObjectPro(TIdentity id)
            : base(id)
        {
            this.OnConstructed();
        }

        /// <summary>
        /// 实体所在的内聚根
        /// </summary>
        [PropertyRepository]
        public TRoot Root
        {
            get
            {
                return GetRoot();
            }
        }

        IAggregateRoot IEntityObjectPro.Root
        {
            get
            {
                return this.Root;
            }
        }


        /// <summary>
        /// 获得实体所在的内聚根
        /// </summary>
        /// <returns></returns>
        protected abstract TRoot GetRoot();


        public override bool Equals(object obj)
        {
            var target = obj as TObject;
            if (target == null) return false;
            return this.UniqueKey == target.UniqueKey;
        }

        public override int GetHashCode()
        {
            return this.UniqueKey.GetHashCode();
        }
    }

}
