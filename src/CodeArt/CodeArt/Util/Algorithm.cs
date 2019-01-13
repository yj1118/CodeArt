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

        /// <summary>
        /// 找出<paramref name="target"/>里的成员的所有组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Compose<T>(this IEnumerable<T> target, Func<List<T>, bool> filter = null)
        {
            if (target.Count() == 0) return Array.Empty<List<T>>();
            if (target.Count() == 1) return new List<List<T>>() { new List<T> { target.First() } };

            var result = new List<List<T>>();

            var all = target.FullArray();
            foreach (var temp in all)
            {
                var items = temp.ComposeForFirst();
                foreach (var item in items)
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

                    if (filter != null && !filter(item)) continue;

                    result.Add(item);
                }
            }
            return result;

            //var temp = target.ToList();
            //var pointer = 0;
            //while (pointer < temp.Count)
            //{
            //    if (pointer == 0)
            //    {
            //        result.AddRange(temp.ComposeForFirst());
            //    }
            //    else
            //    {
            //        var other = temp[pointer];
            //        //调整位置
            //        temp.Remove(other);
            //        temp.Insert(0, other);
            //        result.AddRange(temp.ComposeForFirst());

            //        //还原数组
            //        temp.Remove(other);
            //        temp.Insert(pointer, other);
            //    }
            //    pointer++;
            //}
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
    }
}
