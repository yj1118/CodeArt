using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;


namespace CodeArt.DTO
{
    /// <summary>
    /// 为指定类型上的属性存储序列化信息
    /// </summary>
    internal class MemberSerializationInfoComparator : IComparer<MemberSerializationInfo>
    {
        private MemberSerializationInfoComparator() { }

        public int Compare(MemberSerializationInfo x, MemberSerializationInfo y)
        {
            return string.Compare(x.Name, y.Name, true);
        }

        /// <summary>
        /// 由于该对象只有算法，没有状态，因此所有线程可以共用这一个对象，达到降低内存压力的效果
        /// </summary>
        public static readonly MemberSerializationInfoComparator Instance = new MemberSerializationInfoComparator();

    }
}
