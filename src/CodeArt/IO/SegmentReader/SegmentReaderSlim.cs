using System;
using System.Collections.Concurrent;
using System.Runtime;
using System.IO;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.IO
{
    /// <summary>
    /// 分块读取器，用于将流分块读取，
    /// 如果需要知道块的位置信息，请用SegmentReaderSlim对象
    /// </summary>
    public class SegmentReaderSlim
    {
        private int _bufferSize;
        private Stream _target;
        private byte[] _buffer;
        private long _readCount;

        private SegmentReaderSlim(int bufferSize)
        {
            _bufferSize = bufferSize;
            _buffer = new byte[_bufferSize];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="handle">返回true表示继续处理下一个块，返回false表示中断处理</param>
        public void Read(Stream target, Func<Segment, bool> handle)
        {
            this.Clear();
            _target = target;
            Segment current = Segment.Empty;
            while (true)
            {
                current = Read();
                if (current.IsEmpty()) break;
                if (!handle(current)) break;
            }
        }

        private Segment Read()
        {
            bool isFirst = _readCount == 0;
            int retval = _target.ReadPro(_buffer, 0, _bufferSize);
            _readCount += retval;

            if (retval == 0) return Segment.Empty;
            return new Segment(_buffer, retval);
        }

        private void Clear()
        {
            _target = null;
            _readCount = 0;
        }

        #region 内部类

        public struct Segment
        {
            /// <summary>
            /// 缓冲区
            /// </summary>
            public byte[] Buffer
            {
                get;
                private set;
            }

            /// <summary>
            /// 块的有效长度
            /// </summary>
            public int Length
            {
                get;
                private set;
            }

            /// <summary>
            ///  获取块的有效内容
            /// </summary>
            public byte[] GetContent()
            {
                if (this.Length == 0) return Array.Empty<byte>();
                if (this.Length == this.Buffer.Length) return this.Buffer;
                var content = new byte[this.Length];
                System.Buffer.BlockCopy(this.Buffer, 0, content, 0, this.Length);
                return content;
            }

            internal Segment(byte[] buffer, int length)
            {
                this.Buffer = buffer;
                this.Length = length;
            }

            public bool IsEmpty()
            {
                return this.Buffer == null;
            }

            public static readonly Segment Empty = new Segment(null, 0);

        }

        #endregion

        #region 池构造

        public static IPoolItem<SegmentReaderSlim> Borrow(SegmentSize segmentSize)
        {
            var pool = _getPool(segmentSize.Value);
            return pool.Borrow();
        }

        private static Func<int, Pool<SegmentReaderSlim>> _getPool = LazyIndexer.Init<int, Pool<SegmentReaderSlim>>((bufferSize) =>
        {
            return new Pool<SegmentReaderSlim>(() =>
            {
                return new SegmentReaderSlim(bufferSize);
            }, (reader, phase) =>
            {
                if (phase == PoolItemPhase.Returning)
                {
                    reader.Clear();
                }
                return true;
            }, new PoolConfig());
        });

        #endregion
    }
}
