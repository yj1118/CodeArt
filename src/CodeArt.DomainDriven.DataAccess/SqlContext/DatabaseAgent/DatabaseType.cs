using System;
using System.Collections.Generic;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 内置识别的数据库类型
    /// </summary>
    public static class DatabaseType
    {
        public const string SQLServer = "SQLServer";
        public const string MySQL = "MySQL";
        public const string Oracle = "Oracle";

        /// <summary>
        /// 微软的MicrosoftAccess数据库
        /// </summary>
        public const string Access = "Access";
    }
}