using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 领域缓冲池，用于缓冲领域对象
    /// </summary>
    internal class DomainBuffer : IDisposable
    {
        private IDomainBuffer _buffer;

        public Guid Id = Guid.NewGuid();

        public DomainBuffer()
        {
            _buffer = DomainDrivenConfiguration.Current.BufferConfig.GetCache() ?? new MemoryBuffer();
        }

        /// <summary>
        /// 启用缓冲
        /// </summary>
        private void Enabled()
        {
            DataContext.RolledBack += DataContextRolledBack;
        }

        /// <summary>
        /// 禁用缓冲区
        /// </summary>
        private void Disabled()
        {
            _buffer.Clear();
            DataContext.RolledBack -= DataContextRolledBack;
        }

        private void DataContextRolledBack(object sender, RolledBackEventArgs e)
        {
            //当数据上下文回滚时，我们需要移除数据上下文用到的对象，因为这些对象有可能状态是脏的
            {
                var objs = e.Context.GetBufferObjects();
                foreach (var obj in objs)
                {
                    Remove(obj.UniqueKey);
                }
            }

            {
                var objs = e.Context.GetMirrorObjects();
                foreach (var obj in objs)
                {
                    Remove(obj.UniqueKey);
                }
            }
        }

        #region 在缓冲池中加载或创建对象

        public IAggregateRoot GetOrCreate(Type objectType, object id, int dataVersion, Func<IAggregateRoot> load)
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
        private IAggregateRoot GetOrCreateImpl(string uniqueKey, int dataVersion, Func<IAggregateRoot> load)
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
        public void Add(Type objectType, object id, IAggregateRoot root)
        {
            var uniqueKey = GetUniqueKey(objectType, id);
            Add(uniqueKey, root);
        }

        private void Add(string uniqueKey, IAggregateRoot root)
        {
            var result = new BufferEntry(root); //主动追加的数据，版本号从1开始
            _buffer.AddOrUpdate(uniqueKey, result);
        }

        #endregion

        #region 删除对象

        public void Remove(Type objectType, object id)
        {
            var uniqueKey = GetUniqueKey(objectType, id);
            Remove(uniqueKey);
        }

        private void Remove(string uniqueKey)
        {
            var entry = _buffer.Remove(uniqueKey);
            if(entry != null)
            {
                //缓冲区的数据移除后，也要主动将数据代理给清空
                //因为数据代理可能包含一些线程公共资源，这些资源必须清理，不然下次加载数据，又会使用这些公共资源
                //数据代理中AppSession就是典型的例子
                var obj = entry.Root as DomainObject;
                obj.DataProxy.Clear();
            }
        }

        //清空所有缓冲对象
        public void Clear()
        {
            _buffer.Clear();
        }

        #endregion

        #region 获取key

        private static string GetUniqueKey(Type objectType, object id)
        {
            return UniqueKeyCalculator.GetUniqueKey(objectType, id);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Disabled();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DomainBuffer() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion


        /// <summary>
        /// 公共的对象缓冲池，用于存放非镜像查询得到的对象
        /// </summary>
        public static readonly DomainBuffer Public;


        #region 基于当前应用程序会话的对象换冲池


        private const string _sessionKey = "DomainBuffer.Mirror";

        /// <summary>
        /// 获取或设置当前会话的对象缓冲池，这用于镜像的加载
        /// </summary>
        public static DomainBuffer Mirror
        {
            get
            {
                var buffer = AppSession.GetOrAddItem<DomainBuffer>(
                    _sessionKey,
                    () =>
                    {
                        var _buffer = Symbiosis.TryMark<DomainBuffer>(_mirrors, () => { return new DomainBuffer(); });
                        _buffer.Enabled(); //借出项时，需要启用
                        return _buffer;
                    });
                if (buffer == null) throw new InvalidOperationException(Strings.DomainBufferMirrorNull);
                return buffer;
            }
        }

        //public static bool ExistMirror()
        //{
        //    return AppSession.GetItem<DomainBuffer>(_sessionKey) != null;
        //}


        #endregion

        #region 对象池

        private static Pool<DomainBuffer> _mirrors;


        static DomainBuffer()
        {
            Public = new DomainBuffer();
            Public.Enabled(); //公共缓冲区是永久启用的

            _mirrors = new Pool<DomainBuffer>(() =>
            {
                return new DomainBuffer();
            }, (buffer, phase) =>
            {
                if (phase == PoolItemPhase.Returning)
                {
                    buffer.Disabled(); //返回池时需要禁用缓冲区
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //5分钟之内未被使用，就移除
            });
        }

        #endregion

    }
}