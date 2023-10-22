using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    internal static class SqlStatement
    {
        /// <summary>
        /// 包装标示限定符
        /// </summary>
        /// <returns></returns>
        public static string Qualifier(string name)
        {
            switch(SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer:
                    {
                        if (name.StartsWith("[")) return name;
                        return string.Format("[{0}]", name);
                    }
                case DatabaseType.MySQL:
                    {
                        if (name.StartsWith("`")) return name;
                        return string.Format("`{0}`", name);
                    }
            }
            return name;
        }
    }
}
