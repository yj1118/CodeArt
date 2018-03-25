using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using System.IO;

using System.Text;
using System.Diagnostics;


namespace CodeArt.IO
{
    /// <summary>
    /// 分段大小，我们把零散的分段大小归一化成几个有限的分段大小，这样不仅利于用户更加轻松的使用，还可以提高分段利用率
    /// </summary>
    [DebuggerDisplay("Type = SegmentSize, Size = {Value}")]
    public sealed class SegmentSize
    {
        public int Value
        {
            get;
            private set;
        }

        private SegmentSize(int value)
        {
            this.Value = value;
        }


        /// <summary>
        /// 根据数据长度，获得建议的段大小
        /// </summary>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        public static SegmentSize GetAdviceSize(long dataLength)
        {
            if (dataLength < Byte128.Value) return Byte128;

            //段的数量不能太多，也不能太少，
            //我们假设将数据分为3段，计算出每段的平均大小
            //找出距离平均大小最近的段size
            var average = dataLength / 3;
            if (average > KB256.Value) return KB256;

            foreach (var size in _middleSizes)
            {
                if (size.Value > average) return size;
            }
            return KB256;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as SegmentSize;
            if (target == null) return false;
            return target.Value == this.Value;
        }

        internal static SegmentSize GetInstance(int value)
        {
            switch (value)
            {
                case _Byte128: return Byte128;
                case _Byte256: return Byte256;
                case _Byte512: return Byte512;
                case _KB1: return KB1;
                case _KB2: return KB2;
                case _KB4: return KB4;
                case _KB8: return KB8;
                case _KB16: return KB16;
                case _KB32: return KB32;
                case _KB64: return KB64;
                case _KB128: return KB128;
                case _KB256: return KB256;
            }
            throw new InvalidOperationException("没有找到" + value + "对应的ByteSegmentSize");
        }


        #region 静态成员

        private const int _Byte128 = 128;
        private const int _Byte256 = 256;
        private const int _Byte512 = 512;
        private const int _KB1 = 1024;
        private const int _KB2 = 1024 * 2;
        private const int _KB4 = 1024 * 4;
        private const int _KB8 = 1024 * 8;
        private const int _KB16 = 1024 * 16;
        private const int _KB32 = 1024 * 32;
        private const int _KB64 = 1024 * 64;
        private const int _KB128 = 1024 * 128;
        private const int _KB256 = 1024 * 256;

        public static readonly SegmentSize Byte128 = new SegmentSize(_Byte128);
        public static readonly SegmentSize Byte256 = new SegmentSize(_Byte256);
        public static readonly SegmentSize Byte512 = new SegmentSize(_Byte512);
        public static readonly SegmentSize KB1 = new SegmentSize(_KB1);
        public static readonly SegmentSize KB2 = new SegmentSize(_KB2);
        public static readonly SegmentSize KB4 = new SegmentSize(_KB4);
        public static readonly SegmentSize KB8 = new SegmentSize(_KB8);
        public static readonly SegmentSize KB16 = new SegmentSize(_KB16);
        public static readonly SegmentSize KB32 = new SegmentSize(_KB32);
        public static readonly SegmentSize KB64 = new SegmentSize(_KB64);
        public static readonly SegmentSize KB128 = new SegmentSize(_KB128);
        public static readonly SegmentSize KB256 = new SegmentSize(_KB256);

        /// <summary>
        /// 该数组用于计算分段大小时使用
        /// </summary>
        private static SegmentSize[] _middleSizes = new SegmentSize[] {
            Byte128,Byte256,Byte512,KB1,KB2,KB4,KB8,KB16,KB32,KB64,KB128,KB256
        };

        #endregion
    }
}