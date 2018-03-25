using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using System.IO;

using CodeArt.Concurrent;
using System.Text;

namespace CodeArt.IO
{
    /// <summary>
    /// 字节数组，每一个字节数组由多个分段组成
    /// </summary>
    public class ByteArray : IDisposable
    {
        private List<byte[]> _segments;

        /// <summary>
        /// 最后一个段落已经使用的偏移量,该偏移量的上一个字节是被使用的，偏移量指向的字节是没有被使用的
        /// </summary>
        private int _lastSegmentOffset;

        public int Length
        {
            get;
            private set;
        }

        public SegmentSize SegmentSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前已分配的分段数
        /// </summary>
        public int AllocatedSegmentCount
        {
            get
            {
                return _segments.Count;
            }
        }

        private int _segmentSizeValue;

        internal ByteArray(SegmentSize size)
        {
            this.SegmentSize = size;
            _segmentSizeValue = size.Value;
            _segments = new List<byte[]>();
            _lastSegmentOffset = 0;
            this.Length = 0;
            this.ReadPosition = 0;
        }

        internal ByteArray(int dataLength)
            : this(SegmentSize.GetAdviceSize(dataLength))
        {
            
        }

        #region 写入数据

        /// <summary>
        /// 向数组中写入字节序列
        /// </summary>
        /// <param name="buffer">字节数组。此方法将 count 个字节从 buffer 复制到当前data。</param>
        /// <param name="offset">buffer 中的从零开始的字节偏移量，从此处开始将字节复制到当前data。</param>
        /// <param name="count">要写入当前data的字节数。</param>
        public void Write(byte[] buffer, int offset, int count)
        {
start:
            var last = GetLast();
            var writeCount = count;  //需要写入的字节数
            int canUseCount;
            if (LastSegmentNotEnough(writeCount, out canUseCount))
            {
                Buffer.BlockCopy(buffer, offset, last, _lastSegmentOffset, canUseCount);
                UpdatePosition(canUseCount);

                NewSegment();
                offset += canUseCount;
                count -= canUseCount;
goto start;
                //（解开递归，我们发现如果段数组的字节数比较小，写入的数据量比较大，会递归上几十次甚至上百次，所以用goto语句解开）
                //Write(buffer, offset + canUseCount, count);//继续写入剩余字节数
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, last, _lastSegmentOffset, writeCount);
                UpdatePosition(writeCount);
            }
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public void Write(byte value)
        {
            using (var temp = ByteBuffer.BrrowByte1())
            {
                var bytes = temp.Item;
                bytes[0] = value;
                Write(bytes);
            }
        }

        /// <summary>
        /// 请用该方法写入整型，不会产生4字节临时数组
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            using (var poolItem = ByteBuffer.BrrowByte4())
            {
                var bytes = poolItem.Item;
                value.GetBytes(bytes, 0);
                Write(bytes);
            }
        }

        public void Write(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            this.Write(bytes.Length);
            this.Write(bytes);
        }

        /// <summary>
        /// 请尽量不要使用该方法，因为会产生bytes临时数组
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            var bytes = value.GetBytes(Encoding.UTF8);
            Write(bytes.Length);
            Write(bytes);
        }

        public void Write(string[] values)
        {
            Write(values.Length);
            foreach (var value in values)
            {
                Write(value);
            }
        }

        public void Write(Guid value)
        {
            Write(value.ToByteArray());
        }

        public void Write(DateTime value)
        {
            Write(value.ToBinary());
        }

        /// <summary>
        /// 读取流中的数据，写入到数组中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="count">需要写入的字节数</param>
        public void Write(Stream stream, int count)
        {
start:
            var last = GetLast();
            var writeCount = count;  //需要写入的字节数
            int canUseCount;
            if (LastSegmentNotEnough(writeCount, out canUseCount))
            {
                stream.ReadPro(last, _lastSegmentOffset, canUseCount);
                UpdatePosition(canUseCount);

                var residueWriteCount = writeCount - canUseCount; //剩余的需要写入的字节数
                NewSegment();
                count = residueWriteCount;
goto start;
                //Write(stream, residueWriteCount);//继续写入剩余字节数
            }
            else
            {
                stream.ReadPro(last, _lastSegmentOffset, writeCount);
                UpdatePosition(writeCount);
            }
        }

        /// <summary>
        /// 将目标数组写入到本地数组中，这里是根据target当前的ReadPosition来写入的
        /// </summary>
        /// <param name="target"></param>
        public void Write(ByteArray target)
        {
            if (target == null) return;
            var position = target.ReadPosition;
            target.ReadBytes(this, (target.Length - position));
            target.ReadPosition = position;  //还原目标的读取位置
        }


        /// <summary>
        /// 将目标数组写入到本地数组中，这里是根据target当前的ReadPosition来写入的
        /// </summary>
        /// <param name="target"></param>
        /// <param name="count">需要写入的字节数</param>
        public void Write(ByteArray target, int count)
        {
            if (target == null) return;
            var position = target.ReadPosition;
            target.ReadBytes(this, count);
            target.ReadPosition = position;
        }

        #endregion

        #region 向数据的起始位置插入数据

        /// <summary>
        /// 向数据的起始位置插入数据，返回结果，该方法不会对当前对象造成影响，返回新数据
        /// </summary>
        /// <param name="value"></param>
        public IPoolItem<ByteArray> Insert(int value)
        {
            var bytesTemp = ByteBuffer.Borrow(this.SegmentSize);
            var bytes = bytesTemp.Item;

            bytes.Write(value);
            bytes.Write(this);
            return bytesTemp;
        }


        #endregion

        #region 内部的重构方法

        /// <summary>
        /// 新增字节段
        /// </summary>
        private void NewSegment()
        {
            var seg = ByteBuffer.BrrowSegment(_segmentSizeValue);
            _segments.Add(seg);
            _lastSegmentOffset = 0; //重置最后一个字节段的偏移量
        }

        /// <summary>
        /// 已写入count个字节
        /// </summary>
        /// <param name="count"></param>
        private void UpdatePosition(int count)
        {
            this.Length += count;
            _lastSegmentOffset += count;
        }

        /// <summary>
        /// 判断最后的片段空间是否不足
        /// </summary>
        /// <param name="needCount">需要的字节数</param>
        /// <param name="AvailableSpace">会记录最后片段目前可以使用的空间</param>
        /// <returns></returns>
        private bool LastSegmentNotEnough(int needCount, out int canUseCount)
        {
            var last = GetLast();
            canUseCount = last.Length - _lastSegmentOffset; //最后一个字节段剩余的可用空间d的长度
            return needCount > canUseCount;
        }

        private byte[] GetLast()
        {
            if (_segments.Count == 0) NewSegment();
            return _segments.Last();
        }
        #endregion

        #region 读取数据

        /// <summary>
        /// 读取数组中的内容，写入到流中
        /// </summary>
        /// <param name="target"></param>
        public void Read(Stream target)
        {
            var lastIndex = _segments.Count - 1;
            for (var i = 0; i < _segments.Count; i++)
            {
                var seg = _segments[i];
                if (i == lastIndex)
                {
                    target.Write(seg, 0, _lastSegmentOffset);
                    this.ReadPosition += _lastSegmentOffset;
                }
                else
                {
                    target.Write(seg, 0, seg.Length);
                    this.ReadPosition += seg.Length;
                }
            }
        }

        /// <summary>
        /// 获取数组数据，该方法不会影响ReadPosition
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            if (this.Length == 0) return Array.Empty<byte>();
            var buffer = new byte[this.Length];
            var lastIndex = _segments.Count - 1;
            var offset = 0;
            for (var i = 0; i < _segments.Count; i++)
            {
                var seg = _segments[i];
                if (i == lastIndex)
                {
                    var count = _lastSegmentOffset;
                    Buffer.BlockCopy(seg, 0, buffer, offset, count);
                    offset += count;
                }
                else
                {
                    var count = seg.Length;
                    Buffer.BlockCopy(seg, 0, buffer, offset, count);
                    offset += count;
                }
            }
            return buffer;
        }

        /// <summary>
        /// 当前读取位置的地址，该地址与写入数据无关，写入数据始终在末尾开始写入
        /// </summary>
        public int ReadPosition
        {
            get;
            set;
        }


        /// <summary>
        /// 将字节读入到buffer中
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset">从此处开始写入到buffer</param>
        /// <param name="count">需要读取的字节数</param>
        public void ReadBytes(byte[] buffer, int offset, int count)
        {
start:
            var seg = GetReadSegment();
            int segOffset = GetReadSegmentOffset();
            var canReadCount = _segmentSizeValue - segOffset;
            if (canReadCount < count)
            {
                var readCount = canReadCount;
                Buffer.BlockCopy(seg, segOffset, buffer, offset, readCount);
                this.ReadPosition += readCount;

                //更新读取状态
                offset += readCount;
                count -= readCount;
goto start;
            }
            else
            {
                var readCount = count;
                Buffer.BlockCopy(seg, segOffset, buffer, offset, readCount);
                this.ReadPosition += readCount;
            }
        }

        /// <summary>
        /// 将字节读入到bytes中
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="count">需要写入的字节数</param>
        public void ReadBytes(ByteArray bytes, int count)
        {
            if (bytes == null) return;
start:
            var seg = GetReadSegment();
            int segOffset = GetReadSegmentOffset();
            var canReadCount = _segmentSizeValue - segOffset;
            if (canReadCount < count)
            {
                var readCount = canReadCount;
                bytes.Write(seg, segOffset, readCount);
                this.ReadPosition += readCount;

                count -= readCount;
goto start;
            }
            else
            {
                var readCount = count;
                bytes.Write(seg, segOffset, readCount);
                this.ReadPosition += readCount;
            }
        }

        /// <summary>
        /// 读取指定的字节数以写入字节数组中，并将当前读取位置前移相应的字节数。
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            if (count == 0) return Array.Empty<byte>();

            var bytes = new byte[count];
            ReadBytes(bytes, 0, bytes.Length);
            return bytes;
        }

        public byte ReadByte()
        {
            using (var temp = ByteBuffer.BrrowByte1())
            {
                var bytes = temp.Item;
                ReadBytes(bytes, 0, 1);
                return bytes[0];
            }
        }

        public int ReadInt32()
        {
            int value = 0;
            using (var poolItem = ByteBuffer.BrrowByte4())
            {
                var bytes = poolItem.Item;
                ReadBytes(bytes, 0, bytes.Length);
                value = bytes.ToInt();
            }
            return value;
        }

        public long ReadInt64()
        {
            var length = ReadInt32();
            var bytes = ReadBytes(length);
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary>
        /// 请尽量不要使用该方法，因为会产生bytes临时数组
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            var bytesCount = ReadInt32();
            var bytes = ReadBytes(bytesCount);
            return bytes.GetString(Encoding.UTF8);
        }

        public string[] ReadStrings()
        {
            var count = ReadInt32();
            string[] values = new string[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = ReadString();
            }
            return values;
        }

        public Guid ReadGuid()
        {
            var bytes = this.ReadBytes(16);
            return new Guid(bytes);
        }

        public DateTime ReadDateTime()
        {
            var dateData = this.ReadInt64();
            return DateTime.FromBinary(dateData);
        }

        /// <summary>
        /// 获取当前正在读取的字节段
        /// </summary>
        /// <returns></returns>
        private byte[] GetReadSegment()
        {
            var index = this.ReadPosition / _segmentSizeValue;
            return _segments[index];
        }

        private int GetReadSegmentOffset()
        {
            return this.ReadPosition % _segmentSizeValue; //在当前读取段中的偏移量
        }


        #endregion

        internal void Clear()
        {
            ByteBuffer.ReturnSegments(_segmentSizeValue, _segments);
            _segments.Clear();
            this.Length = 0;
            _lastSegmentOffset = 0;
            this.ReadPosition = 0;
        }

        public void Dispose()
        {
            this.Clear();
        }
    }
}