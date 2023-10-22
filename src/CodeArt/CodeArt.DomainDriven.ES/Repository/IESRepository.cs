using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.Search;
using CodeArt.Concurrent;
using System.Configuration;

namespace CodeArt.DomainDriven.DataAccess
{
    public interface IESRepository
    {
        /// <summary>
        /// 恢复没有处理的事务
        /// </summary>
        void Restore();
    }
}
