using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.DomainDriven.DataAccess.SQLServer
{
    internal static class Util
    {
        public static string GetSqlDbTypeString(DbType dbType)
        {
            return _getSqlDbTypeString(dbType);
        }

        private static Func<DbType, string> _getSqlDbTypeString = LazyIndexer.Init<DbType, string>((dbType) =>
        {
            SqlParameter p = new SqlParameter();
            p.DbType = dbType;
            return p.SqlDbType.ToString().ToLower();
        });


        public static SqlDbType GetSqlDbType(DbType dbType)
        {
            return _getSqlDbType(dbType);
        }


        private static Func<DbType, SqlDbType> _getSqlDbType = LazyIndexer.Init<DbType, SqlDbType>((dbType) =>
        {
            SqlParameter p = new SqlParameter();
            p.DbType = dbType;
            return p.SqlDbType;
        });



        //private static System.Data.DbType SqlDbType2DbType(System.Data.SqlDbType pSourceType)
        //{
        //    SqlParameter paraConver = new SqlParameter();
        //    paraConver.SqlDbType = pSourceType;
        //    return paraConver.DbType;
        //}

        //private static string GetDbTypeString(DbType dbType)
        //{
        //    switch (dbType)
        //    {
        //        case DbType.AnsiString: return "varchar";
        //        case DbType.Binary: return "varbinary";
        //        case DbType.Byte: return "tinyint";
        //        case DbType.Boolean: return "bit";
        //        case DbType.Currency: return "money";
        //        case DbType.Date: return "datetime";
        //        case DbType.DateTime: return "datetime";
        //        case DbType.Time: return "datetime";
        //        case DbType.DateTime2: return "datetime2";
        //        case DbType.DateTimeOffset: return "datetimeoffset";

        //        case DbType.Decimal: return "decimal";
        //        case DbType.Double: return "float";
        //        case DbType.Guid: return "uniqueidentifier";
        //        case DbType.Int16: return "smallInt";
        //        case DbType.Int32: return "int";

        //        case DbType.Int64: return "bigint";
        //        case DbType.Object: return "variant";
        //        case DbType.Single: return "real";
        //        case DbType.String: return "nvarchar";
        //        case DbType.AnsiStringFixedLength: return "char";
        //        case DbType.StringFixedLength: return "nchar";
        //        case DbType.Xml: return "xml";

        //    }
        //    throw new DataAccessException(string.Format(Strings.NotSupportDbType, dbType.ToString()));
        //}
    }
}
