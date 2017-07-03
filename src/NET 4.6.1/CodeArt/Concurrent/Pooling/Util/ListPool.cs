using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Concurrent
{
    public static class ListPool<T>
    {
        public static IPoolItem<List<T>> Borrow()
        {
            return Instance.Borrow();
        }


        public static readonly Pool<List<T>> Instance = new Pool<List<T>>(() =>
        {
            return new List<T>();
        }, (list, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                list.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

    }
}
