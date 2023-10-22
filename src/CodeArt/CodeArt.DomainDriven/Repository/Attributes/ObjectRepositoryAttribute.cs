using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示对象是可以被仓储的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ObjectRepositoryAttribute : RepositoryAttribute
    {
        /// <summary>
        /// 该对象所用到的仓储接口的类型
        /// </summary>
        public Type RepositoryInterfaceType
        {
            get;
            private set;
        }


        public Type ObjectType
        {
            get;
            internal set;
        }

        /// <summary>
        /// 还是否开启了快照
        /// </summary>
        public bool Snapshot
        {
            get;
            set;
        }


        /// <summary>
        /// 快照的寿命，单位天，该值小于0则为不保存快照，等于0表示快照永久存在，大于0为快照的保存时间
        /// 目前只支持不保存快照和永久保存
        /// </summary>
        public int SnapshotLifespan
        {
            get
            {
                return this.Snapshot ? 0 : -1;
            }
        }

        /// <summary>
        /// 关闭多租户功能，这意味着即使配置文件开启了多租户特性，目标对象也不会启动多租户功能
        /// </summary>
        public bool CloseMultiTenancy
        {
            get;
            set;
        }

        /// <summary>
        /// 表示对象可以仓储，仓储的接口类型为所在聚合根的仓储的类型
        /// </summary>
        public ObjectRepositoryAttribute()
            : this(null)
        {
        }

        public ObjectRepositoryAttribute(Type repositoryInterfaceType)
        {
            this.RepositoryInterfaceType = repositoryInterfaceType;
            this.Snapshot = false;
            this.CloseMultiTenancy = false;
        }


        public static ObjectRepositoryAttribute GetTip(Type objectType, bool checkUp)
        {
            var attr = _getTip(objectType);
            if (attr == null && checkUp)
                throw new DomainDrivenException(string.Format(Strings.NotDefinedObjectRepositoryAttribute, objectType.FullName));
            return attr;
        }


        private static Func<Type, ObjectRepositoryAttribute> _getTip = LazyIndexer.Init<Type, ObjectRepositoryAttribute>((objectType) =>
        {
            var attr = objectType.GetCustomAttribute<ObjectRepositoryAttribute>(true);
            if (attr == null) return null;
            if (attr.ObjectType == null)
            {
                attr.ObjectType = objectType; //填充对象类型信息
            }
            return attr;
        });



    }
}
