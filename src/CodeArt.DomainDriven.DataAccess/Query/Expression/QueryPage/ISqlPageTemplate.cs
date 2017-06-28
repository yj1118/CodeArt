using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 为翻页语句提供模板
    /// </summary>
    public interface ISqlPageTemplate
    {
        /// <summary>
        /// 模板根据传递的页面序号和页大小获取翻页代码
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        string GetCode(int pageIndex, int pageSize);
    }
}
