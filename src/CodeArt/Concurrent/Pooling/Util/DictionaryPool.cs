using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Concurrent
{
    public static class DictionaryPool<TKey, TValue>
    {
        public static IPoolItem<Dictionary<TKey, TValue>> Borrow()
        {
            return Instance.Borrow();
        }


        public static readonly Pool<Dictionary<TKey, TValue>> Instance = new Pool<Dictionary<TKey, TValue>>(() =>
        {
            return new Dictionary<TKey, TValue>();
        }, (data, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                data.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });
    }


    public static class DictionaryPoolWrapper<TKey, TValue>
    {
        public static Dictionary<TKey, TValue> Borrow()
        {
            return Instance.Borrow();
        }

        public static void Return(Dictionary<TKey, TValue> item)
        {
            Instance.Return(item);
        }


        private static readonly PoolWrapper<Dictionary<TKey, TValue>> Instance = new PoolWrapper<Dictionary<TKey, TValue>>(() =>
        {
            return new Dictionary<TKey, TValue>();
        }, (data, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                data.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });
    }


}
