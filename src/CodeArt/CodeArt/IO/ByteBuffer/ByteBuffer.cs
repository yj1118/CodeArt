using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using System.IO;
using System.Text;

using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.IO
{
    /// <summary>
    /// 从字节缓冲区中获得字节数组，重复使用已分配的字节流
    /// </summary>
    public class ByteBuffer : IDisposable
    {
        private LazyIndexer<SegmentSize, ByteArrayPool> _pools = new LazyIndexer<SegmentSize, ByteArrayPool>((size) =>
        {
            return new ByteArrayPool(size);
        }, (pool) =>
        {
            //返回true表示不过滤
            return true;
        });

        private ByteBuffer()
        {
        }

        /// <summary>
        /// 使用方式:
        /// using(var temp = buffer.GetBytes(ByteSegmentSize.Byte256))
        /// {
        ///     var bytes = temp.Items;
        /// }
        /// </summary>
        /// <param name="segmentSize"></param>
        /// <returns></returns>
        private IPoolItem<ByteArray> BorrowBytes(SegmentSize segmentSize)
        {
            var pool = _pools.Get(segmentSize);
            return pool.Borrow();
        }

        /// <summary>
        /// 根据数据的大小，自动判断段大小
        /// </summary>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        private IPoolItem<ByteArray> BorrowBytes(int dataLength)
        {
            return BorrowBytes(SegmentSize.GetAdviceSize(dataLength));
        }

        public void Dispose()
        {
            var values = _pools.Values;
            foreach (var value in values)
                value.Dispose();
            _pools.Clear();
        }

        #region 借/还 ByteArray

        /// <summary>
        /// 全局的字节缓冲区
        /// </summary>
        private static ByteBuffer Global = new ByteBuffer();

        /// <summary>
        /// 从字节缓冲区中借用字节数组
        /// </summary>
        /// <param name="segmentSize"></param>
        /// <returns></returns>
        public static IPoolItem<ByteArray> Borrow(SegmentSize segmentSize)
        {
            return Global.BorrowBytes(segmentSize);
        }

        /// <summary>
        /// 从字节缓冲区中借用字节数组
        /// </summary>
        /// <param name="dataLength">用于存放数据的长度，该长度可以是实际存放数据的近似值，用于生成段大小</param>
        /// <returns></returns>
        public static IPoolItem<ByteArray> Borrow(int dataLength)
        {
            return Global.BorrowBytes(dataLength);
        }

        #endregion

        #region 1字节池

        /// <summary>
        /// 4字节池，用于读写数据时，提供临时的4字节空间
        /// </summary>
        private static Pool<byte[]> _byte1Pool;

        internal static IPoolItem<byte[]> BrrowByte1()
        {
            return _byte1Pool.Borrow();
        }

        internal static void ReturnByte1(IPoolItem<byte[]> item)
        {
            _byte1Pool.Return(item);
        }

        #endregion;


        #region 4字节池

        /// <summary>
        /// 4字节池，用于读写数据时，提供临时的4字节空间
        /// </summary>
        private static Pool<byte[]> _byte4Pool;

        internal static IPoolItem<byte[]> BrrowByte4()
        {
            return _byte4Pool.Borrow();
        }

        internal static void ReturnByte4(IPoolItem<byte[]> item)
        {
            _byte4Pool.Return(item);
        }

        #endregion;

        #region 分段池

        internal static byte[] BrrowSegment(int segmentSize)
        {
            var pool = _getSegmentPool(segmentSize);
            return pool.Borrow();
        }

        internal static void ReturnSegment(int segmentSize, byte[] item)
        {
            var pool = _getSegmentPool(segmentSize);
            pool.Return(item);
        }

        internal static void ReturnSegments(int segmentSize, IList<byte[]> items)
        {
            var pool = _getSegmentPool(segmentSize);
            foreach(var item in items)
            {
                pool.Return(item);
            }
        }

        private static Func<int, PoolWrapper<byte[]>> _getSegmentPool = LazyIndexer.Init<int, PoolWrapper<byte[]>>((segmentSize) =>
        {
            return new PoolWrapper<byte[]>(() =>
            {
                return new byte[segmentSize];
            }, (reader, phase) =>
            {
                return true;
            }, new PoolConfig() { MaxRemainTime = 600 }); //闲置10分钟销毁
        });

        #endregion

        static ByteBuffer()
        {
            _byte1Pool = new Pool<byte[]>(() =>
            {
                return new byte[1];
            }, (reader, phase) =>
            {
                return true;
            }, new PoolConfig() { MaxRemainTime = 600 }); //闲置10分钟销毁

            _byte4Pool = new Pool<byte[]>(() =>
            {
                return new byte[4];
            }, (reader, phase) =>
            {
                return true;
            }, new PoolConfig() { MaxRemainTime = 600 }); //闲置10分钟销毁
        }

    }
}