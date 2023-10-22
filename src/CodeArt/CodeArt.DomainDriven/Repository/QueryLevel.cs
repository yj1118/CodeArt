using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 查询级别
    /// </summary>
    public sealed class QueryLevel
    {
        public int Code
        {
            get;
            private set;
        }

        private QueryLevel(int code)
        {
            Code = code;
        }

        internal const int NoneCode = 1;
        internal const int ShareCode = 2;
        internal const int SingleCode = 3;
        internal const int HoldSingleCode = 4;
        internal const int MirroringCode = 5;

        /// <summary>
        /// 无锁
        /// </summary>
        public static readonly QueryLevel None = new QueryLevel(NoneCode);

        /// <summary>
        /// 只有一个线程可以访问查询的结果集，其余线程将等待
        /// </summary>
        public static readonly QueryLevel Single = new QueryLevel(SingleCode);

        /// <summary>
        /// 只有一个线程可以访问查询的结果集或者满足查询条件的不存在的数据，其余线程将等待
        /// 也就是说，HoldSingle锁可以保证满足查询条件的现有数据和即将插入的数据都被锁住
        /// 开启互斥锁和HoldSingle
        /// 可以防止在查询中别的线程插入数据
        /// </summary>
        public static readonly QueryLevel HoldSingle = new QueryLevel(HoldSingleCode);

        /// <summary>
        /// 共享锁，当前线程开启此线程后，不会影响其他线程获取Share、None
        /// 但是其他线程不能立即获取Single或HoldSingle锁，需要等待只读线程操作完成后才行；
        /// 另外，当其他线程正在Single或者HoldSingle,当前线程也无法获得只读锁，需要等待
        /// </summary>
        public static readonly QueryLevel Share = new QueryLevel(ShareCode);

        /// <summary>
        /// 以镜像的形式加载对象，该模式下不会从缓冲区中获取对象而是直接以无锁的模式加载全新的对象
        /// </summary>
        public static readonly QueryLevel Mirroring = new QueryLevel(MirroringCode);


        public static implicit operator int(QueryLevel level)
        {
            return level.Code;
        }

        public static bool operator ==(QueryLevel level0, QueryLevel level1)
        {
            if ((object)level0 == null && (object)level1 == null) return true;
            if ((object)level0 == null || (object)level1 == null) return false;
            return level0.Code == level1.Code;
        }

        public static bool operator !=(QueryLevel level0, QueryLevel level1)
        {
            return !(level0 == level1);
        }

        public override bool Equals(object obj)
        {
            QueryLevel target = obj as QueryLevel;
            if (target == null) return false;
            return this.Code == target.Code;
        }

        public override int GetHashCode()
        {
            return this.Code;
        }


        public string GetMSSqlLockCode()
        {
            switch (this.Code)
            {
                case QueryLevel.ShareCode: return string.Empty;
                case QueryLevel.SingleCode: return " with(xlock,rowlock) ";
                case QueryLevel.HoldSingleCode: return " with(xlock,holdlock) ";
                default:
                    return " with(nolock) "; //None和Mirror 都是无锁模式
            }
        }
    }
}