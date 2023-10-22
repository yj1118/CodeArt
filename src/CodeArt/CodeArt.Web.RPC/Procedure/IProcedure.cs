using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;
using CodeArt.Security;
using CodeArt.AppSetting;

namespace CodeArt.Web.RPC
{
    public interface IProcedure
    {
        string Path
        {
            get;
            set;
        }

        ILogExtractor LogExtractor
        {
            get;
            set;
        }

        IEnumerable<AuthAttribute> Auths
        {
            get;
        }

        DTObject Invoke(DTObject arg);

        string GetCacheKey(DTObject arg);

        ICache GetCache();


        /// <summary>
        /// 获得相应结果的代码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        string GetResponseCode(DTObject result);

        DTObject GetLogContent(DTObject arg);

        /// <summary>
        /// 获得需要验证授权的数据信息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        DTObject GetAuthData(DTObject arg);
    }
}
