using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeArt.DomainDriven.DataAccess.SQLServer
{
    internal class SqlInsertBuilder
    {
        private StringBuilder _names = new StringBuilder();
        private StringBuilder _paras = new StringBuilder();
        private string _tbName;

        public SqlInsertBuilder(string tbName)
        {
            _tbName = tbName;
        }

        public void AddField(string field)
        {
            _names.AppendFormat("[{0}],", field);
            _paras.AppendFormat("@{0},", field);
        }

        public string GetCommandText()
        {
            string sql = string.Empty;
            if (_names.Length > 0)
            {
                _names.Length --;
                _paras.Length --;
                sql = string.Format("insert into [{0}]({1}) values({2})", _tbName, _names.ToString(), _paras.ToString());
                //还原状态
                _names.Append(",");
                _paras.Append(",");
            }
            return sql;
        }

        public override string ToString()
        {
            return GetCommandText();
        }

        /// <summary>
        /// 重置sql语句内容
        /// </summary>
        /// <param name="name"></param>
        public void Reset(string tbName)
        {
            _tbName = tbName;
            _names.Clear();
            _paras.Clear();
        }

    }
}
