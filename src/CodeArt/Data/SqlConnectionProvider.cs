using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using CodeArt.Util;

namespace CodeArt.Data
{
    public class SqlConnectionProvider : ISqlConnectionProvider
    {
        protected SqlConnectionProvider() { }

        public virtual string GetConnectionString(string connName)
        {
            return _getConnectionString(connName);
        }

        private static Func<string, string> _getConnectionString = LazyIndexer.Init<string, string>((connName) =>
        {
            if (!connName.StartsWith("db-")) connName = string.Format("db-{0}", connName);
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[connName];
            if (setting == null)
            {
                setting = ConfigurationManager.ConnectionStrings["db-default"];
                if (setting == null) throw new DataException("没有找到名称为" + connName + "或 db-default 的连接配置");
            }
            return setting.ConnectionString;
        });

        public static SqlConnectionProvider Instance = new SqlConnectionProvider();

    }
}