using CodeArt.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven.DataAccess
{
    internal sealed class SqlColumns
    {
        public IEnumerable<string> Select
        {
            get;
            private set;
        }

        public IEnumerable<string> Where
        {
            get;
            private set;
        }

        public IEnumerable<string> Order
        {
            get;
            private set;
        }

        /// <summary>
        /// 判定查询是否涉及到<paramref name="fieldName"/>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool Contains(string fieldName)
        {
            if (this.IsAll) return true;

            return this.Select.Contains(fieldName, StringComparer.OrdinalIgnoreCase)
                    || this.Where.Contains(fieldName, StringComparer.OrdinalIgnoreCase)
                    || this.Order.Contains(fieldName, StringComparer.OrdinalIgnoreCase);
        }


        public bool IsAll
        {
            get;
            private set;
        }


        public SqlColumns(IEnumerable<string> select, IEnumerable<string> where, IEnumerable<string> order)
        {
            this.Select = Map(select); //将对象关系链改成 _
            this.Where = Map(where);
            this.Order = Map(order);
            this.IsAll = this.Select.Contains("*");
        }

        private static IEnumerable<string> Map(IEnumerable<string> columns)
        {
            return columns.Select((t) => t.Replace(".", "_"));
        }


        public static readonly SqlColumns Empty = new SqlColumns(EmptyArray<string>.Value,
                                                                EmptyArray<string>.Value,
                                                                EmptyArray<string>.Value);

        public static readonly SqlColumns All = new SqlColumns(new string[] { "*" },
                                                        EmptyArray<string>.Value,
                                                        EmptyArray<string>.Value);


    }
}
