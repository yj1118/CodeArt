using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 类型索引表
    /// </summary>
    internal sealed class TypeIndex
    {
        private Dictionary<string, TypeEntry> _index = new Dictionary<string, TypeEntry>(StringComparer.OrdinalIgnoreCase);

        private Func<string, string> _getCompleteName;

        //private Func<string, IEnumerable<string>> _getScopes = LazyIndexer.Init<string, IEnumerable<string>>((currentScope) =>
        //{
        //    var scopes = new List<string>();//例如：Admin.person.hourse 会收集到 Admin.person.hourse、Admin.person、Admin
        //    var temp = currentScope;
        //    while (true)
        //    {
        //        scopes.Add(temp);
        //        int pos = temp.LastIndexOf(".");
        //        if (pos == -1) break;
        //        temp = temp.Substring(0, pos);
        //    }
        //    return scopes;
        //});

        private string _rootTypeName;
        private string _rootTypeNameDot;


        public TypeIndex()
        {
            _getCompleteName = LazyIndexer.Init<string, string>((typeName) =>
            {
                if (typeName.StartsWith(_rootTypeNameDot)) return typeName;  //已经包含根路径了
                return string.Format("{0}.{1}", _rootTypeName, typeName);
            });
        }

        public void Add(TypeEntry entry)
        {
            if (!_index.ContainsKey(entry.TypeName))
                _index.Add(entry.TypeName, entry);

            if(_index.Count == 1)
            {
                _rootTypeName = entry.Name; //记录根类型的名称
                _rootTypeNameDot = string.Format("{0}.", _rootTypeName);
            }
        }

        public bool TryGet(string typeName, out TypeEntry entry)
        {
            if (_index.TryGetValue(typeName, out entry)) return true;
            if (string.IsNullOrEmpty(_rootTypeName)) return false;
            //因为有可能成员采用的简写（不带根的名称）,所以我们需要用完整名称再匹配一次
            var completeName = _getCompleteName(typeName);
            if (completeName.Length == typeName.Length) return false;
            return _index.TryGetValue(completeName, out entry);
        }

        public bool Contains(string typeName)
        {
            return _index.ContainsKey(typeName);
        }

        /// <summary>
        /// 在作用域范围内查找匹配的entry
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        //public bool TryGet(string currentScope, string typeName, out TypeEntry entry)
        //{
        //    if (TryGet(typeName, out entry)) return true; //完整匹配
        //    if (currentScope.IndexOf(".") == -1) return false;

        //    //在作用域范围内找
        //    var scopes = _getScopes(currentScope);
        //    foreach(var scope in scopes)
        //    {
        //        var targetType = string.Format("{0}.{1}", scope, typeName);
        //        if (TryGet(targetType, out entry))
        //            return true;
        //    }
        //    return false;
        //}




    }
}
