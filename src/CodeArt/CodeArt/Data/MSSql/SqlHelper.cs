using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


using CodeArt.Util;

namespace CodeArt.Data.MSSql
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class SqlHelper
    {
        #region 参数

        private static Func<Type, SqlDbType> _getDbType = LazyIndexer.Init<Type, SqlDbType>((type) =>
        {
            if (type == typeof(Guid)) return SqlDbType.UniqueIdentifier;
            if (type == typeof(bool)) return SqlDbType.Bit;
            if (type == typeof(int)) return SqlDbType.Int;
            if (type == typeof(string)) return SqlDbType.NVarChar;
           
            if (type == typeof(byte)) return SqlDbType.TinyInt;
            if (type == typeof(DateTime)) return SqlDbType.DateTime;
            if (type == typeof(decimal)) return SqlDbType.Decimal;
            if (type == typeof(double)) return SqlDbType.Float;

            if (type == typeof(short)) return SqlDbType.SmallInt;
            if (type == typeof(long)) return SqlDbType.BigInt;

            if (type == typeof(float)) return SqlDbType.Real;
            if (type == typeof(byte[])) return SqlDbType.VarBinary;
            if (type == typeof(char)) return SqlDbType.Char;
            if (type == typeof(object)) return SqlDbType.Variant;


            throw new DataException("没有找到类型" + type.FullName + "对应的数据库字段类型");
        });

        public static SqlParameter CreateParameter(string name, object value, SqlDbType dbType)
        {
            SqlParameter prm = new SqlParameter(name, dbType);
            prm.Value = value;
            return prm;
        }

        public static SqlParameter CreateParameter(string name, object value)
        {
            if (value == null) throw new DataException("sql参数" + name + "不能为null");
            SqlParameter prm = new SqlParameter(name, _getDbType(value.GetType()));
            prm.Value = value;
            return prm;
        }

        public static SqlParameter CreateParameter(string name)
        {
            SqlParameter prm = new SqlParameter();
            prm.ParameterName = name;
            return prm;
        }

        public static SqlParameter[] CreateParameters(Action<SqlParameterCollector> collect)
        {
            SqlParameterCollector t = new SqlParameterCollector();
            collect(t);
            SqlParameter[] prms = new SqlParameter[t.Count];
            for (var i = 0; i < t.Count; i++)
            {
                prms[i] = CreateParameter(t.GetName(i), t.GetValue(i),t.GetDbType(i));
            }
            return prms;
        }

        public sealed class SqlParameterCollector
        {
            private List<string> _names = new List<string>();
            private List<object> _values = new List<object>();
            private List<SqlDbType> _dbTypes = new List<SqlDbType>();

            public void Add(string name, object value)
            {
                if (value == null) throw new DataException("sql参数" + name + "不能为null");
                Add(name,value,_getDbType(value.GetType()));
            }

            public void AddLike(string name, string value)
            {
                Add(name, string.Format("%{0}%", value), SqlDbType.NVarChar);
            }

            public void AddLike(string name, string value,SqlDbType dbType)
            {
                Add(name, string.Format("%{0}%", value), dbType);
            }

            public bool TryAdd(string name, object value)
            {
                if (value == null) return false;
                Add(name, value, _getDbType(value.GetType()));
                return true;
            }

            public void Add(string name, object value,SqlDbType dbType)
            {
                _names.Add(name);
                _values.Add(value);
                _dbTypes.Add(dbType);
            }


            public int Count
            {
                get
                {
                    return _names.Count;
                }
            }

            internal string GetName(int i) { return _names[i]; }
            internal object GetValue(int i) { return _values[i]; }
            internal SqlDbType GetDbType(int i) { return _dbTypes[i]; }

            public SqlParameter[] GetParameters()
            {
                SqlParameter[] prms = new SqlParameter[this.Count];
                for (var i = 0; i < this.Count; i++)
                {
                    prms[i] = CreateParameter(this.GetName(i), this.GetValue(i), this.GetDbType(i));
                }
                return prms;
            }
        }

        public static string WrapByLike(string name)
        {
            return string.Format("%{0}%", name);
        }


        #endregion

        #region ExecuteCommandText

        public static SqlData ExecuteCommandText(string connName, string commandText, params SqlParameter[] prms)
        {
            return Execute(connName, commandText, CommandType.Text, prms);
        }

        public static SqlData ExecuteCommandText(string connName, string commandText)
        {
            return Execute(connName, commandText, CommandType.Text, null);
        }

      
        #endregion

        #region ExecuteStoredProcedure

        public static SqlData ExecuteStoredProcedure(string connName, string commandText, params SqlParameter[] prms)
        {
            return Execute(connName, commandText, CommandType.StoredProcedure, prms);
        }

        public static SqlData ExecuteStoredProcedure(string connName, string commandText)
        {
            return Execute(connName, commandText, CommandType.StoredProcedure, null);
        }

        #endregion

        public static string GetConnectionString(string connName)
        {
            var provider = SqlContext.GetConnectionProvider();
            return provider.GetConnectionString(connName);
        }

        public static bool ExistConnection(string connName)
        {
            var provider = SqlContext.GetConnectionProvider();
            return provider.ExistConnectionString(connName);
        }

        private static SqlData Execute(string connName, string commandText, CommandType cmdType, params SqlParameter[] prms)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(GetConnectionString(connName)))
            {
                SqlCommand cmd = new SqlCommand(commandText, sqlConn);
                cmd.CommandType = cmdType;
                if (prms != null)
                    cmd.Parameters.AddRange(prms);

                sqlConn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            return new SqlData(ds);
        }

        public static string GetLockCode(SqlLockLevel level)
        {
            switch (level)
            {
                case SqlLockLevel.ShareLock: return string.Empty;
                case SqlLockLevel.XLock: return "with(xlock,rowlock)";
                case SqlLockLevel.HoldLock: return "with(xlock,holdlock)";
                default:
                    return "with(nolock)";
            }
        }
    
    }
}
