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

        public bool Snapshot
        {
            get
            {
                return this.SnapshotLifespan >= 0;
            }
        }


        /// <summary>
        /// 快照的寿命，单位天，该值小于0则为不保存快照，等于0表示快照永久存在，大于0为快照的保存时间
        /// </summary>
        public int SnapshotLifespan
        {
            get;
            set;
        }

        #region 缓存设置

        /// <summary>
        /// 缓存策略
        /// </summary>
        public CachePolicy CachePolicy
        {
            get;
            set;
        }

        /// <summary>
        /// 指示是否永久缓存
        /// </summary>
        public bool NotRemovable
        {
            get
            {
                return this.CachePolicy.NotRemovable;
            }
            set
            {
                this.CachePolicy.NotRemovable = value;
            }
        }

        /// <summary>
        /// 指示是否应在给定时段内未访问（单位分钟），就会对它进行是否逐出缓存项。
        /// </summary>
        public double SlidingExpiration
        {
            get
            {
                return this.CachePolicy.SlidingExpiration.TotalMinutes;
            }
            set
            {
                this.CachePolicy.SlidingExpiration = TimeSpan.FromMinutes(value);
            }
        }

        /// <summary>
        /// 指示项不被缓存
        /// </summary>
        public bool NoCache
        {
            get
            {
                return this.CachePolicy.NoCache;
            }
            set
            {
                this.CachePolicy.NoCache = value;
            }
        }

        #endregion


        public ObjectRepositoryAttribute(Type repositoryInterfaceType)
        {
            this.RepositoryInterfaceType = repositoryInterfaceType;
            this.SnapshotLifespan = -1;
            this.CachePolicy = new CachePolicy();
        }


        public static ObjectRepositoryAttribute GetTip(Type objectType, bool checkUp)
        {
            var attr = _getTip(objectType);
            if (attr == null && checkUp) throw new DomainDrivenException(string.Format(Strings.NotDefinedObjectRepositoryAttribute, objectType.FullName));
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
