using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CodeArt.DomainDriven.DataAccess
{
    public class SqlConnectionProvider : ISqlConnectionProvider
    {
        protected SqlConnectionProvider() { }

        public virtual string GetConnectionString(string connName)
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[connName];
            if (setting == null)
            {
                setting = ConfigurationManager.ConnectionStrings["db-default"];
                if (setting == null) throw new DataException("没有找到名称为" + connName + "或 db-default 的连接配置");
            }
            return setting.ConnectionString;
        }

        public static readonly SqlConnectionProvider Instance = new SqlConnectionProvider();

    }
}