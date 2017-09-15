using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 查询表达式
    /// </summary>
    internal class QueryExpression
    {
        /// <summary>
        /// 是否查询自身的成员
        /// </summary>
        public bool IsSelfEntities
        {
            get;
            private set;
        }

        /// <summary>
        /// 该表达式对应的路径片段
        /// </summary>
        public string Segment
        {
            get;
            private set;
        }

        /// <summary>
        /// 表达式的下一个查询表达式
        /// </summary>
        public QueryExpression Next
        {
            get;
            private set;
        }

        public bool IsEmpty
        {
            get;
            private set;
        }


        private QueryExpression(string queryString)
        {
            this.IsEmpty = string.IsNullOrEmpty(queryString);
            this.IsSelfEntities = queryString == "*";
            this.Segment = GetSegment(queryString);
            this.Next = GetNext(queryString);
        }

        private string GetSegment(string queryString)
        {
            var segment = queryString;
            int dot = queryString.IndexOf('.');
            if (dot > -1) segment = queryString.Substring(0, dot);
            return segment;
        }


        private QueryExpression GetNext(string queryString)
        {
            int dot = queryString.IndexOf('.');
            if (dot == -1) return null;
            var nextQueryString = queryString.Substring(dot + 1);
            return QueryExpression.Create(nextQueryString);
        }


        #region 静态成员

        private static Func<string, QueryExpression> _getExpression = LazyIndexer.Init<string, QueryExpression>((queryString) =>
        {
            return new QueryExpression(queryString);
        });

        public static QueryExpression Create(string queryString)
        {
            return _getExpression(queryString);
        }

        #endregion
    }
}
