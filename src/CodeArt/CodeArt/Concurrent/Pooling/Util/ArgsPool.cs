using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Concurrent
{
    public static class ArgsPool
    {
        private static Pool<object[]> _args1Pool = new Pool<object[]>(() =>
        {
            return new object[] { null };
        }, (args, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                args[0] = null;
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        /// <summary>
        /// 得到参数数量为2的参数对象
        /// </summary>
        /// <returns></returns>
        public static IPoolItem<object[]> Borrow1()
        {
            return _args1Pool.Borrow();
        }


        private static Pool<object[]> _args2Pool = new Pool<object[]>(() =>
        {
            return new object[] { null, null };
        }, (args, phase) =>
        {
            if(phase == PoolItemPhase.Returning)
            {
                args[0] = null;
                args[1] = null;
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        /// <summary>
        /// 得到参数数量为2的参数对象
        /// </summary>
        /// <returns></returns>
        public static IPoolItem<object[]> Borrow2()
        {
            return _args2Pool.Borrow();
        }

    }
}
