using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 领域缓冲池，用于缓冲领域对象
    /// </summary>
    internal static class DomainBuffer
    {
        private static IDomainBuffer _buffer;

        static DomainBuffer()
        {
            _buffer = DomainDrivenConfiguration.Current.BufferConfig.GetCache() ?? new MemoryBuffer();
            DataContext.RolledBack += DataContextRolledBack;
        }

        private static void DataContextRolledBack(object sender, RolledBackEventArgs e)
        {
            //当数据上下文回滚时，我们需要移除数据上下文用到的对象，因为这些对象有可能状态是脏的
            var objs = e.Context.GetBufferObjects();
            foreach (var obj in objs)
            {
                Remove(obj.UniqueKey);
            }
        }

        #region 在缓冲池中加载或创建对象

        public static IAggregateRoot GetOrCreate(Type objectType, object id, int dataVersion, Func<IAggregateRoot> load)
        {
            var uniqueKey = GetUniqueKey(objectType, id);
            return GetOrCreateImpl(uniqueKey, dataVersion, load);
        }

        /// <summary>
        /// 从缓存区中创建或者获取数据
        /// </summary>
        /// <param name="tip"></param>
        /// <param name="getCacheKey"></param>
        /// <param name="dataVersion"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        private static IAggregateRoot GetOrCreateImpl(string uniqueKey, int dataVersion, Func<IAggregateRoot> load)
        {
            BufferEntry result = null;
            if (_buffer.TryGet(uniqueKey, out result))
            {
                if (result.DataVersion == dataVersion)
                    return result.Root;
            }

            //更新缓冲区
            var root = load();
            result = new BufferEntry(root);
            _buffer.AddOrUpdate(uniqueKey, result);
            return result.Root;
        }

        #endregion

        #region 将对象加入容器

        /// <summary>
        /// 将对象加入缓存
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(Type objectType, object id, IAggregateRoot root)
        {
            var uniqueKey = GetUniqueKey(objectType, id);
            Add(uniqueKey, root);
        }

        private static void Add(string uniqueKey, IAggregateRoot root)
        {
            var result = new BufferEntry(root); //主动追加的数据，版本号从1开始
            _buffer.AddOrUpdate(uniqueKey, result);
        }

        #endregion

        #region 删除对象

        public static void Remove(Type objectType, object id)
        {
            var uniqueKey = GetUniqueKey(objectType, id);
            Remove(uniqueKey);
        }

        private static void Remove(string uniqueKey)
        {
            _buffer.Remove(uniqueKey);
        }

        #endregion

        #region 获取key

        private static string GetUniqueKey(Type objectType, object id)
        {
            return UniqueKeyCalculator.GetUniqueKey(objectType, id);
        }

        #endregion

    }
}