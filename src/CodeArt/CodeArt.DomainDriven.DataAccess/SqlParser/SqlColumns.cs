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
            return this.IsAll || Specified(fieldName);
        }

        /// <summary>
        /// 确实手工指定了某个字段,与Contains不同,当 select * 时，Contains返回的是true
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool Specified(string fieldName)
        {
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


        public static readonly SqlColumns Empty = new SqlColumns(Array.Empty<string>(),
                                                                Array.Empty<string>(),
                                                                Array.Empty<string>());

        public static readonly SqlColumns All = new SqlColumns(new string[] { "*" },
                                                        Array.Empty<string>(),
                                                        Array.Empty<string>());


    }
}
