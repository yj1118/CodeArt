using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using CodeArt.Concurrent;


namespace CodeArt.Util
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HashCoder<TObject>
    {
        private static int _typeHashCode;
        private static bool _isPrimitiveType;

        static HashCoder()
        {
            _isPrimitiveType = DataUtil.IsPrimitiveType(typeof(TObject));
            _typeHashCode = typeof(TObject).GetHashCode();
        }

        public static int GetCode(TObject obj)
        {
            if (_isPrimitiveType) return obj.GetHashCode();
            if (obj == null) return 0;
            return obj.GetHashCode();
        }

        public static int GetCode(IEnumerable<TObject> objs)
        {
            if(objs == null) return 0;
            using (var temp = ListPool<int>.Borrow())
            {
                var list = temp.Item;
                foreach(var obj in objs)
                {
                    list.Add(GetCode(obj));
                }
                return Combine(list);
            }
        }

        /// <summary>
        /// 通过多个hashCode得到唯一的hashCode
        /// </summary>
        /// <param name="hashCodes"></param>
        /// <returns></returns>
        public static int Combine(params int[] hashCodes)
        {
            return Combine(hashCodes.AsEnumerable());
        }

        public static int Combine(IEnumerable<int> hashCodes)
        {
            if (hashCodes.Count() == 0) return _typeHashCode;
            var result = _typeHashCode;
            foreach (var code in hashCodes)
            {
                result = (((result << 5) + result) ^ code);//这是微软的
            }
            return result;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HashCoder
    {
        public static int Combine(params int[] hashCodes)
        {
            return Combine(hashCodes);
        }

        public static int Combine(IEnumerable<int> hashCodes)
        {
            var count = hashCodes.Count();
            if (count == 0) return 0;
            if (count == 1) return hashCodes.First();
            var result = 0;
            foreach (var code in hashCodes)
            {
                result = (((result << 5) + result) ^ code);  //这是微软的
            }
            return result;
        }

        public static int GetCode(object obj)
        {
            if (obj == null) return 0;
            var list = obj as IEnumerable;
            if (list != null) GetCode(list);
            return obj.GetHashCode();
        }

        public static int GetCode(IEnumerable objs)
        {
            if (objs == null) return 0;
            return GetCode((hashCodes) =>
            {
                foreach (var obj in objs)
                {
                    hashCodes.Add(GetCode(obj));
                }
            });
        }


        public static int GetCode(Type objectType, Action<List<int>> collect)
        {
            using (var temp = ListPool<int>.Borrow())
            {
                var list = temp.Item;
                if (objectType != null) list.Add(objectType.GetHashCode());
                collect(list);
                return Combine(list);
            }
        }

        public static int GetCode(Action<List<int>> collect)
        {
            return GetCode(null, collect);
        }
    }

    //外国网友提供的
    //public static int ComputeHashFrom(params object[] obj)
    //{
    //    ulong res = 0;
    //    for (uint i = 0; i < obj.Length; i++)
    //    {
    //        object val = obj[i];
    //        res += val == null ? i : (ulong)val.GetHashCode() * (1 + 2 * i);
    //    }
    //    return (int)(uint)(res ^ (res >> 32));
    //}

    //以下是java的
    //public int hashCode()
    //{
    //    final int prime = 31;
    //    int result = 1;
    //    result = prime * result + a;
    //    return result;
    //}

    //微软字符串的拼接算法
    //internal static uint ComputeStringHash(string s)
    //{
    //    uint num;
    //    if (s != null)
    //    {
    //        num = 0x811c9dc5;
    //        for (int i = 0; i < s.Length; i++)
    //        {
    //            num = (s[i] ^ num) * 0x1000193;
    //        }
    //    }
    //    return num;
    //}

    //微软Array的内部算法
    //internal static int CombineHashCodes(int h1, int h2)
    //{
    //    return (((h1 << 5) + h1) ^ h2);
    //}



}
