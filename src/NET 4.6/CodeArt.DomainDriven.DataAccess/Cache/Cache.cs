using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    internal static class Cache
    {
        private static ICache _cache;

        static Cache()
        {
            _cache = DataAccessConfiguration.Current.GetCache() ?? new MemoryCacheProxy();
        }


        #region 加载或创建缓存对象

        public static object GetOrCreate(ObjectRepositoryAttribute tip, object id, int dataVersion, Func<object> load)
        {
            return GetOrCreateImpl(tip, () =>
            {
                return GetCacheKey(tip, id);
            }, dataVersion, load);
        }

        public static object GetOrCreate(ObjectRepositoryAttribute tip, object rootId, object id, int dataVersion, Func<object> load)
        {
            return GetOrCreateImpl(tip, ()=>
            {
                return GetCacheKey(tip, rootId, id);
            }, dataVersion, load);
        }

        /// <summary>
        /// 从缓存区中创建或者获取数据
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="getCacheKey"></param>
        /// <param name="dataVersion"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        private static object GetOrCreateImpl(ObjectRepositoryAttribute tip, Func<string> getCacheKey, int dataVersion, Func<object> load)
        {
            if (tip.NoCache) return load();

            var cacheKey = getCacheKey();
            CacheEntry result = null;
            if (_cache.TryGet(tip.CachePolicy, cacheKey, out result))
            {
                if (result.DataVersion >= dataVersion)
                    return result.Object;
            }

            //更新缓存
            var obj = load();
            result = new CacheEntry(obj, dataVersion);
            _cache.AddOrUpdate(tip.CachePolicy, cacheKey, result);
            return result.Object;
        }

        #endregion

        #region 将对象加入缓存

        /// <summary>
        /// 将对象加入缓存
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(ObjectRepositoryAttribute tip, object rootId, object id, DomainObject obj)
        {
            var cacheKey = GetCacheKey(tip, rootId, id);
            Add(tip.CachePolicy, cacheKey, obj);
        }

        /// <summary>
        /// 将对象加入缓存
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(ObjectRepositoryAttribute tip, object id, DomainObject obj)
        {
            var cacheKey = GetCacheKey(tip, id);
            Add(tip.CachePolicy, cacheKey, obj);
        }

        private static void Add(CachePolicy policy, string cacheKey, object obj)
        {
            var result = new CacheEntry(obj, 1); //主动追加的数据，版本号从1开始
            _cache.AddOrUpdate(policy, cacheKey, result);
        }

        #endregion

        //#region 修改缓存

        ///// <summary>
        /////  该方法并不会修改缓存，只会同步对象的一致性，因为被修改的对象就是从缓存区中得到的，不需要再更新
        /////  ，如果对象是NoCache，那更加不需要更新缓存
        ///// </summary>
        ///// <param name="tip"></param>
        ///// <param name="rootId"></param>
        ///// <param name="id"></param>
        ///// <param name="obj"></param>
        //public static void Update(ObjectRepositoryAttribute tip, object rootId, object id, DomainObject obj)
        //{
        //    var cacheKey = GetCacheKey(tip, rootId, id);
        //    BufferChannel.Expired(cacheKey, obj);
        //}

        ///// <summary>
        /////  该方法并不会修改缓存，只会同步对象的一致性，因为被修改的对象就是从缓存区中得到的，不需要再更新
        /////  ，如果对象是NoCache，那更加不需要更新缓存
        ///// </summary>
        ///// <param name="tip"></param>
        ///// <param name="id"></param>
        ///// <param name="obj"></param>
        //public static void Update(ObjectRepositoryAttribute tip, object id, DomainObject obj)
        //{
        //    var cacheKey = GetCacheKey(tip, id);
        //    BufferChannel.Expired(cacheKey, obj);
        //}

        //#endregion


        #region 删除缓存

        public static void Remove(ObjectRepositoryAttribute tip, object id)
        {
            var cacheKey = GetCacheKey(tip, id);
            Remove(tip, cacheKey);
        }

        public static void Remove(ObjectRepositoryAttribute tip, object rootId, object id)
        {
            var cacheKey = GetCacheKey(tip, rootId, id);
            Remove(tip, cacheKey);
        }

        private static void Remove(ObjectRepositoryAttribute tip, string cacheKey)
        {
            _cache.Remove(tip.CachePolicy, cacheKey);
        }


        #endregion

        #region 获取缓存key

        private static string GetCacheKey(ObjectRepositoryAttribute tip, object id)
        {
            return string.Format("{0}+{1}", tip.ObjectType.FullName, id.ToString());
        }

        private static string GetCacheKey(ObjectRepositoryAttribute tip, object rootId, object id)
        {
            return string.Format("{0}+{1}+{2}", tip.ObjectType.FullName, rootId.ToString(), id.ToString());
        }

        #endregion

    }
}