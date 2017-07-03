using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDriven.DataAccess.SQLServer
{
    /// <summary>
    /// 为翻页语句提供模板
    /// </summary>
    internal class SqlPageTemplate : ISqlPageTemplate
    {
        public SqlPageTemplate()
        {
        }

        private string _select = string.Empty;

        public void Select(string format, params object[] args)
        {
            _select = string.Format(format, args);
        }

        private string _from = string.Empty;
        public void From(string format, params object[] args)
        {
            _from = string.Format(format, args);
        }

        private StringBuilder _condition = new StringBuilder();
        public void Where(string format, params object[] args)
        {
            string cmd = _condition.Length > 0 ? " and " : " where ";
            _condition.Append(cmd);
            _condition.AppendFormat(format, args);
        }

        private string _orderBy = string.Empty;
        public void OrderBy(string format, params object[] args)
        {
            _orderBy = string.Format(format, args);
        }

        private string _groupBy = string.Empty;
        public void GroupBy(string format, params object[] args)
        {
            _groupBy = string.Format(format, args);
        }

        private string GetFirstPageCT()
        {
            return string.Format("select top @data_length * from {0} {1} {2};", GetFrom(), _condition, _orderBy);
        }

        private string GetPageCT()
        {
            string from = GetFrom();

            string aSql = string.Format("select top @data_length {0} as pk,row_number() over({1}) as ind from {2} {3} {1}", EntityObject.IdPropertyName, _orderBy, from, _condition);
            string tSql = string.Format("select pk from ({0}) as a where a.ind>@data_offset and a.ind<=(@data_offset+@data_length)", aSql);

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select * from");
            sql.AppendFormat("    ({0})", tSql);
            sql.AppendLine();
            sql.AppendLine("    as t");
            sql.AppendLine("inner join");
            sql.AppendFormat("    (select * from {0})", from);
            sql.AppendLine();
            sql.AppendLine("     as t2");
            sql.AppendFormat("on t.pk=t2.{0}", EntityObject.IdPropertyName);
            sql.AppendFormat(" {0};", _orderBy);
            return sql.ToString();
        }

        private string GetFrom()
        {
            return string.Format("(select {0} from {1} {2}) as _tb", _select, _from, GetGroupBy());
        }

        private string GetGroupBy()
        {
            if (string.IsNullOrEmpty(_groupBy)) return string.Empty;
            return string.Format("group by {0}", _groupBy);
        }

        #region 模板代码

        private string _templateCode;
        public string TemplateCode
        {
            get
            {
                if(_templateCode == null)
                {
                    _templateCode = GetTemplateCode();
                }
                return _templateCode;
            }
        }

        public string GetTemplateCode()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("if(@data_offset=0)");
            sql.AppendLine(GetFirstPageCT());
            sql.AppendLine("else");
            sql.Append(GetPageCT());


            sql.Replace("@data_offset", "{0}");//替换成格式化参数
            sql.Replace("@data_length", "{1}");

            return sql.ToString();
        }

        /// <summary>
        /// 获取翻页代码
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public string GetCode(int pageIndex, int pageSize)
        {
            int offset = pageIndex * pageSize; //数据的偏移量，为从第几条（offset+1）记录开始，
            int count = pageSize;              //count为返回的记录条数。例：offset:2, count:5 即意为从第3条记录开始的5条记录。
            return string.Format(this.TemplateCode, offset, count);
        }


        #endregion

        public override string ToString()
        {
            return this.TemplateCode;
        }
    }
}
