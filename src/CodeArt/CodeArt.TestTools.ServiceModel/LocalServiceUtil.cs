using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 调用本地服务的工具类
    /// </summary>
    public static class LocalServiceUtil
    {
        /// <summary>
        /// 以dto的形式调用服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="fillArg"></param>
        /// <returns></returns>
        public static DTObject Invoke(string serviceName, Action<DTObject> fillArg)
        {
            var arg = DTObject.Create();
            if (fillArg != null) fillArg(arg);
            ServiceRequest request = new ServiceRequest(serviceName, DTObject.Empty, arg);
            var provider = ServiceProviderFactory.Create(request);
            return provider.Invoke(request);
        }

        /// <summary>
        /// 以动态参数的形式调用服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="fillArg"></param>
        /// <returns></returns>
        public static dynamic DynamicInvoke(string serviceName, Action<dynamic> fillArg)
        {
            return Invoke(serviceName, (arg) =>
            {
                fillArg((dynamic)arg);
            });
        }
    }
}
