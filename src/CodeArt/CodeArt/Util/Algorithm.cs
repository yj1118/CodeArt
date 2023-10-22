using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace CodeArt.Util
{
    public static class Algorithm
    {
        public static T Balance<T>(IList<T> values)
        {
            if (values.Count == 0) return default(T);
            if (values.Count == 1) return values[0];
            var index = new Random().Next(0, values.Count);
            return values[index];
        }

        private readonly static string[] AB = new string[] { "a", "b" };

        public static string BalanceAB()
        {
            return Balance(AB);
        }

        //1,2, 3,4,5
        //2,1,3,4,5
        //3,1,2,

        //[1,2]
        //[1,3]
        //[1,4]
        //[1,5]


        //[1,2,3]
        //[1,2,4]
        //[1,2,5]
        //[1,2,3,4]
        //[1,2,3,5]
        //[1,2,3,4,5]

        //[1,3,2]
        //[1,3,4]
        //[1,3,5]


        //[2,1]
        //[2,3]

        public static IEnumerable<IEnumerable<T>> Compose<T>(this IEnumerable<T> target, Func<IEnumerable<T>, bool> filter = null)
        {
            return _Compose<T,object>(target, null,null, filter);
        }

        public static IEnumerable<IEnumerable<T>> Compose<T,TKey>(this IEnumerable<T> target,int length, Func<T, TKey> keySelector, Func<List<T>, bool> filter = null)
        {
            return _Compose<T, TKey>(target, length, keySelector, filter);
        }

        public static IEnumerable<IEnumerable<T>> Compose<T>(this IEnumerable<T> target, int length, Func<IEnumerable<T>, bool> filter = null)
        {
            return _Compose<T, object>(target, length, null, filter);
        }

        /// <summary>
        /// 找出<paramref name="target"/>里的成员的所有组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="target"></param>
        /// <param name="length">这个参数指的是组合的长度，如果是2，就表示只保留[1,2]、[1,3]等2维组合</param>
        /// <param name="keySelector">指定排序键，一旦指定了该参数，那就意味着组合无视位置，也就是说，1,2组合和2,1组合是同样的，只会保留其中一种</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<T>> _Compose<T, TKey>(this IEnumerable<T> target,int? length, Func<T, TKey> keySelector, Func<List<T>, bool> filter)
        {
            if (target.Count() == 0) return Array.Empty<List<T>>();
            if (target.Count() == 1) return new List<List<T>>() { new List<T> { target.First() } };

            var result = new List<List<T>>();

            if(length != null && length==1)
            {
                foreach (var t in target)
                {
                    var item = new List<T> { t };
                    if (filter != null && !filter(item)) continue;
                    result.Add(item);
                }
                return result;
            }
            else
            {
                var all = target.FullArray();
                foreach (var temp in all)
                {
                    var items = temp.ComposeForFirst();
                    foreach (var item in items)
                    {
                        if (length != null && item.Count != length.Value) continue; //如果指定了数组长度，那么需要判断

                        if (keySelector != null)
                        {
                            //过滤重复,与位置无关
                            var orderItem = item.OrderBy(keySelector).ToArray();
                            if (result.FirstOrDefault((t) =>
                            {
                                if (t.Count != orderItem.Length) return false;
                                var orderT = t.OrderBy(keySelector).ToArray();

                                for (var i = 0; i < orderT.Length; i++)
                                {
                                    if (!orderT[i].Equals(orderItem[i]))
                                        return false;
                                }
                                return true;
                            }) != null) continue;
                        }
                        else
                        {
                            //过滤重复
                            if (result.FirstOrDefault((t) =>
                            {
                                if (t.Count != item.Count) return false;

                                for (var i = 0; i < t.Count; i++)
                                {
                                    if (!t[i].Equals(item[i]))
                                        return false;
                                }
                                return true;
                            }) != null) continue;
                        }

                        if (filter != null && !filter(item)) continue;

                        result.Add(item);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 找出<paramref name="target"/>里的第一个成员为起始项的所有组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        private static IEnumerable<List<T>> ComposeForFirst<T>(this IEnumerable<T> target)
        {
            if (target.Count() == 0) return Array.Empty<List<T>>();
            if (target.Count() == 1) return new List<List<T>>() { new List<T> { target.First() } };

            var temp = target.ToArray();
            var result = new List<List<T>>();

            for (var pointer = 0; pointer < temp.Length; pointer++)
            {
                var root = temp.Take(pointer + 1).ToList();
                for (var index = pointer + 1; index < temp.Length; index++)
                {
                    var current = temp[index];
                    var item = root.ToList();
                    item.Add(current);
                    result.Add(item);
                }
            }

            return result;
        }

        #region 全排列

        public static IEnumerable<List<T>> FullArray<T>(this IEnumerable<T> arr)
        {
            return FullArray(arr.ToList());
        }

        private static List<List<T>> FullArray<T>(this List<T> arr)
        {
            if (arr.Count == 0)
            {
                return new List<List<T>>();
            }

            if (arr.Count == 1)
            {
                return new List<List<T>>() { new List<T>() { arr[0] } };
            }

            if (arr.Count == 2)
            {
                return new List<List<T>>() { new List<T>() { arr[0], arr[1] }, new List<T>() { arr[1], arr[0] } };
            }

            var values = new List<List<T>>();
            for (var i = 0; i < arr.Count; i++)
            {
                var tmp = new List<T>(arr);
                var takeOut = arr[i];
                arr.RemoveAt(i);

                if (arr.Count > 1)
                {
                    var childR = FullArray(arr);
                    var result = FullArray_GetResult(takeOut, childR);
                    values.AddRange(result);
                }

            }

            return values;
        }


        private static List<List<T>> FullArray_GetResult<T>(T val, List<List<T>> oldResult)
        {
            var result = new List<List<T>>();
            for (var i = 0; i < oldResult.Count; i++)
            {
                var temp = FullArray_GetResult(val, oldResult[i]);
                foreach (var item in temp)
                {
                    result.Add(item);
                }

            }
            return result;
        }


        private static List<List<T>> FullArray_GetResult<T>(T val, List<T> old)
        {
            var result = new List<List<T>>();

            for (var i = 0; i <= old.Count; i++)
            {
                var tmp = new List<T>(old);
                tmp.Insert(i, val);
                result.Add(tmp);
            }

            return result;

        }

        #endregion

        /// <summary>
        /// 组合，从多个列表中，组合每项元素，形成新的列表
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static IEnumerable<object> GroupCompose(this IEnumerable<IEnumerable<object>> group)
        {
            if (group.Count() == 0)
                return null;
            int size = 1;
            for (int i = 0; i < group.Count(); i++)
            {
                size = size * group.ElementAt(i).Count();
            }
            object[] result = new object[size];
            for (int j = 0; j < size; j++)
            {
                for (int m = 0; m < group.Count(); m++)
                {
                    if (result[j] == null) result[j] = new List<object>();
                    var current = result[j] as List<object>;
                    var item = group.ElementAt(m).ElementAt((j * Index(group, m) / size) % group.ElementAt(m).Count());
                    current.Add(item);
                }
            }
            return result;


            int Index(IEnumerable<IEnumerable<object>> g, int m)
            {
                int index = 1;
                for (int i = 0; i < g.Count(); i++)
                {
                    if (i <= m)
                    {
                        index = index * g.ElementAt(i).Count();
                    }
                    else
                    {
                        break;
                    }
                }
                return index;
            }
        }
    }
}
