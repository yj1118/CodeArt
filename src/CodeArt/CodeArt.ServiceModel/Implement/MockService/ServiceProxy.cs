using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;

using CodeArt;
using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.ServiceModel.Mock
{
    [SafeAccess]
    public sealed class ServiceProxy : IServiceProxy
    {
        public ServiceProxy() { }

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <param name="address"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public ServiceResponse Invoke(ServiceRequest request)
        {
            var package = ServiceConfiguration.Current.GetContractPackage();
            var contract = package.Find(request);
            return contract.Invoke();
        }

        #region 静态成员

        public static readonly ServiceProxy Instance = new ServiceProxy();

        /// <summary>
        /// 重置mock服务相关的数据，使其恢复到初始状态
        /// </summary>
        public static void Reset()
        {
            var package = ServiceConfiguration.Current.GetContractPackage();
            package.Reset();
        }

        #endregion


    }
}
