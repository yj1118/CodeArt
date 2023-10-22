using CodeArt.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.CI.SqlServer
{
    public static class SqlCMD
    {
        /// <summary>
        /// 完整备份
        /// </summary>
        /// <param name="database">需要备份的数据库</param>
        /// <param name="targetFile">备份生成的文件</param>
        /// <param name="server">数据库服务器的名称或地址</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        public static (string Log, int Code) FullBackup(string database, string targetFile, string server, string user, string password)
        {
            return CMD.Execute(string.Format(@"sqlcmd -S {0} -U {1} -P {2} -Q ""Backup Database {3} To Disk = '{4}'""", server, user, password, database, targetFile));
        }

        /// <summary>
        /// 增量备份
        /// </summary>
        /// <param name="database">需要备份的数据库</param>
        /// <param name="targetFile">备份生成的文件(文件名请自行处理增量序号的问题)</param>
        /// <param name="server">数据库服务器的名称或地址</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        public static (string Log, int Code) IncBackup(string database, string targetFile, string server, string user, string password)
        {
            return CMD.Execute(string.Format(@"sqlcmd -S {0} -U {1} -P {2} -Q ""Backup Log {3} To Disk = '{4}'""", server, user, password, database, targetFile));
        }

        /// <summary>
        /// 执行sql脚本
        /// </summary>
        /// <param name="targetFile">sql脚本所在目录</param>
        /// <param name="server">数据库服务器的名称或地址</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static (string Log, int Code) ExecuteSQL(string database, string targetFile, string server, string user, string password)
        {
            return CMD.Execute(string.Format(@"sqlcmd -S {0} -U {1} -P {2} -d {3} -i {4}", server, user, password, database, targetFile));
        }


    }
}
