using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    internal class ObjectChain
    {
        private IDataField _source;

        private Stack<IDataField> _path;

        public string PathCode
        {
            get;
            private set;
        }

        private Func<DataTable, string> _getPathCode;

        public ObjectChain(IDataField source)
        {
            _source = source;
            _path = GetPath(source);
            this.PathCode = GetPathCode();
            _getPathCode = LazyIndexer.Init<DataTable, string>(GetPathCodeImpl);
        }

        private static Stack<IDataField> GetPath(IDataField source)
        {
            Stack<IDataField> path = new Stack<IDataField>();

            if (source == null) return path;

            var pointer = source;
            while (pointer != null)
            {
                //以下代码是防止死循环的关键代码
                //我们会将当前的pointer与this.MemberField做比较，检查是否为同一个引用点
                //如果是，那么就清理之前的记录的路径path，重新记录
                //这样就可以避免类似ment.parent.parent.parent的死循环了
                if (IsRepeatedReferencePoint(pointer, source))
                {
                    path.Clear();
                }

                path.Push(pointer);
                pointer = pointer.ParentMemberField;
            }
            return path;
        }

        /// <summary>
        /// 是否为重复引用点
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool IsRepeatedReferencePoint(IDataField a, IDataField b)
        {
            if (a == b) return false; //a == b表示两者是同一个field，但不是重复的引用点
            //如果成员字段对应的属性名、成员字段对应的表名、成员字段所在的表的名称相同，那么我们认为是同一个引用点
            return a.GetPropertyName() == b.GetPropertyName()
                    && a.TableName == b.TableName
                    && a.MasterTableName == b.MasterTableName;
        }


        /// <summary>
        /// 获得相对于表<paramref name="parent"/>的对象链的路径代码
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public string GetPathCode(DataTable parent)
        {
            if (_path.Count == 0) return string.Empty;
            return _getPathCode(parent);
        }


        private string GetPathCodeImpl(DataTable parent)
        {
            if (_path.Count == 0) return string.Empty;

            StringBuilder code = new StringBuilder();
            var temp = _path.Reverse();
            foreach (var item in temp)
            {
                if (item == parent.MemberField) break;
                code.Insert(0, string.Format("{0}_", item.GetPropertyName()));
            }
            if (code.Length > 0) code.Length--;
            return code.ToString();
        }


        /// <summary>
        /// 获取自身的全路径
        /// </summary>
        /// <returns></returns>
        private string GetPathCode()
        {
            StringBuilder code = new StringBuilder();
            foreach (var item in _path)
            {
                code.AppendFormat("{0}_", item.GetPropertyName());
            }
            if (code.Length > 0) code.Length--;
            return code.ToString();
        }


        public static readonly ObjectChain Empty = new ObjectChain(null);

    }
}
