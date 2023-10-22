using CodeArt.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.CI.SqlServer
{
    public static class SqlPackage
    {
        /// <summary>
        /// 提取数据库架构文件
        /// </summary>
        /// <param name="targetFile">架构文件存储的路径</param>
        /// <returns></returns>
        public static (string Log, int Code) Extract(string databaseName, string targetFile, string server, string user, string password)
        {
            IOUtil.CreateFileDirectory(targetFile);
            var cmd = string.Format(@"sqlpackage.exe /TargetFile:""{0}"" /Action:Extract /SourceServerName:""{1}"" /SourceUser:""{2}"" /SourcePassword:""{3}"" /SourceDatabaseName:""{4}""", targetFile, server, user, password, databaseName);
            return CMD.Execute(cmd);
        }

        /// <summary>
        /// 生成更新数据库需要运行的脚本
        /// </summary>
        /// <param name="sourceFile">源架构文件的存放路径</param>
        /// <param name="targetFile">需要更新的架构文件的存放路径</param>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns></returns>
        public static (string Log, int Code) Script(string sourceFile, string targetFile, string databaseName, string outputPath)
        {
            var cmd = string.Format(@"sqlpackage.exe /Action:Script /SourceFile:""{0}"" /TargetFile:""{1}"" /TargetDatabaseName:""{2}"" /OutputPath:""{3}"" /p:BlockOnPossibleDataLoss=FALSE /p:AllowIncompatiblePlatform=TRUE", sourceFile, targetFile, databaseName, outputPath);
            return CMD.Execute(cmd);
        }

    }
}
