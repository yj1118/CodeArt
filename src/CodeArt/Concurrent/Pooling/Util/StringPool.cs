using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Concurrent
{
    public static class StringPool
    {
        private static Pool<StringBuilder> _pool = new Pool<StringBuilder>(() =>
       {
           return new StringBuilder();
       }, (str, phase) =>
       {
           if (phase == PoolItemPhase.Returning)
           {
               str.Clear();
           }
           return true;
       }, new PoolConfig()
       {
           MaxRemainTime = 300 //闲置时间300秒
        });

        public static IPoolItem<StringBuilder> Borrow()
        {
            return _pool.Borrow();
        }
    }
}
