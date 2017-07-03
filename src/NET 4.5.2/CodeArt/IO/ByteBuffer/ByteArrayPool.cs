using System;
using System.Collections.Generic;
using System.Runtime;
using System.IO;

using CodeArt.Concurrent;

namespace CodeArt.IO
{
    /// <summary>
    /// <para>为了解决字节数组频繁创建和回收的开销，我们设计了ByteArray和ByteArrayPool对象</para>
    /// <para>ByteArrayPool对象可以提供重复使用的ByteSegment对象，用完ByteArray后请归还到池中</para>
    /// <para>ByteArray对象内部维护多个字节数组，这些字节数组是由ByteArrayPool内部的PoolWrapper<byte[]>提供的</para>
    /// <para>ByteArray对象会根据实际字节数的大小，动态从池中获取字节数组，并内部维护实际长度</para>
    /// <para>在使用的时候，请注意设置segmentSize，当经常接收较大的字节数时，请设置segmentSize为一个较大值</para>
    /// <para>当经常接收较小的字节数时，请设置segmentSize为一个较小值</para>
    /// </summary>
    internal sealed class ByteArrayPool : IDisposable
    {
        /// <summary>
        /// 可伸缩的数组池
        /// </summary>
        private Pool<ByteArray> _bytesPool;

        private SegmentSize _size;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segmentSize">每个数组段的大小，当经常接收较大的字节数时，请设置segmentSize为一个较大值，当经常接收较小的字节数时，请设置segmentSize为一个较小值</param>
        /// <param name="idleTime">限制时间,单位 秒，如果数组段等缓存的对象闲置超过闲置时间，那么销毁池对象</param>
        internal ByteArrayPool(SegmentSize size)
        {
            _size = size;
            _bytesPool = new Pool<ByteArray>(() =>
            {
                return new ByteArray(_size.Value);
            }, (bytes, phase) =>
            {
                bytes.Clear();
                return true;
            }, new PoolConfig() { MaxRemainTime = 600 });
        }

        public IPoolItem<ByteArray> Borrow()
        {
            return _bytesPool.Borrow();
        }

        public void Return(IPoolItem<ByteArray> data)
        {
            _bytesPool.Return(data);
        }

        public void Dispose()
        {
            _bytesPool.Dispose();
            _bytesPool = null;
        }
    }
}
