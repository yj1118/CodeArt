using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeArt.DomainDriven.DataAccess
{
    [DebuggerDisplay("{GetCommandText()}")]
    internal class SqlUpdateBuilder
    {
        private StringBuilder _sql;
        private bool _setFielded;
        private bool _setWhered;

        public SqlUpdateBuilder()
        {
            _sql = new StringBuilder();
            _setFielded = false;
            _setWhered = false;
        }

        public void SetTable(string tbName)
        {
            _sql.AppendFormat("update {0} set", SqlStatement.Qualifier(tbName));
        }

        public void AddField(string field)
        {
            this.Set(string.Format("{0}=@{1}", SqlStatement.Qualifier(field), field));
        }

        public void Set(string setSql)
        {
            if (_setFielded)
            {
                _sql.AppendFormat(",{0}", setSql);
                return;
            }
            _sql.AppendFormat(" {0}", setSql);
            _setFielded = true;
        }

        public void Where(params string[] fields)
        {
            foreach(var field in fields)
            {
                string cmd = _setWhered ? " and " : " where ";
                _sql.Append(cmd);
                _sql.AppendFormat("{0}=@{1}", SqlStatement.Qualifier(field), field);
                _setWhered = true;
            }
        }

        /// <summary>
        /// 该sql语句是否能执行
        /// </summary>
        /// <returns></returns>
        public bool CanPerform()
        {
            return _setFielded;
        }

        public string GetCommandText()
        {
            return _sql.ToString();
        }

        public override string ToString()
        {
            return GetCommandText();
        }


        public void Clear()
        {
            _sql.Clear();
            _setFielded = false;
            _setWhered = false;
        }

    }
}
