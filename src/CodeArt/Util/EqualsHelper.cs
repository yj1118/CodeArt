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
    public static class EqualsHelper
    {
        public static bool ObjectEquals<TObject>(TObject obj1, TObject obj2)
        {
            return Object.Equals(obj1, obj2);
        }

        public static bool ListEquals<TObject>(TObject[] list1, TObject[] list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null) return false;
            if (list2 == null) return false;
            if (list1.Length != list2.Length) return false;

            for (var i = 0; i < list1.Length; i++)
            {
                var obj1 = list1[i];
                var obj2 = list2[i];
                if (!EqualsHelper.ObjectEquals<TObject>(obj1, obj2)) return false;
            }
            return true;
        }


        public static bool ObjectEquals(object obj1, object obj2)
        {
            var list1 = obj1 as IList;
            var list2 = obj2 as IList;
            if (list1 != null && list2 != null) return ListEquals(list1, list2);

            var dictionary1 = obj1 as IDictionary;
            var dictionary2 = obj2 as IDictionary;
            if (dictionary1 != null && dictionary2 != null) return DictionaryEquals(dictionary1, dictionary2);

            return Object.Equals(obj1, obj2);
        }

        //public static bool ListEquals(object[] list1, object[] list2)
        //{
        //    if (list1 == null && list2 == null) return true;
        //    if (list1 == null || list2 != null) return false;
        //    if (list1 != null || list2 == null) return false;
        //    if (list1.Length != list2.Length) return false;

        //    for (var i = 0; i < list1.Length; i++)
        //    {
        //        var obj1 = list1[i];
        //        var obj2 = list2[i];
        //        if (!EqualsHelper.ObjectEquals(obj1, obj2)) return false;
        //    }
        //    return true;
        //}

        public static bool ListEquals(IList list1, IList list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null) return false;
            if (list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            for (var i = 0; i < list1.Count; i++)
            {
                var obj1 = list1[i];
                var obj2 = list2[i];
                if (!EqualsHelper.ObjectEquals(obj1, obj2)) return false;
            }
            return true;
        }

        public static bool DictionaryEquals(IDictionary dictionary1, IDictionary dictionary2)
        {
            if(dictionary1 == null && dictionary2 == null) return true;
            if (dictionary1 == null) return false;
            if (dictionary2 == null) return false;
            if (dictionary1.Count != dictionary2.Count) return false;

            using (var temp1 = ListPool<DictionaryEntry>.Borrow())
            {
                var list1 = temp1.Item;
                foreach(DictionaryEntry p in dictionary1)
                {
                    list1.Add(p);
                }

                using (var temp2 = ListPool<DictionaryEntry>.Borrow())
                {
                    var list2 = temp2.Item;
                    foreach (DictionaryEntry p in dictionary2)
                    {
                        list2.Add(p);
                    }

                    for (var i = 0; i < list1.Count; i++)
                    {
                        var p1 = list1[i];
                        var p2 = list2[i];
                        if (!EqualsHelper.ObjectEquals(p1.Key, p2.Key)) return false;
                        if (!EqualsHelper.ObjectEquals(p1.Value, p2.Value)) return false;
                    }

                }
            } 
            return true;
        }

    }
}
