using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

using CodeArt;

namespace CodeArt.ServiceModel
{
    public static class ServiceProxy
    {

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <param name="address"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static ServiceResponse Invoke(ServiceRequest request)
        {
            return Implement.Invoke(request);
        }

        #region 实现的接口

        private static IServiceProxy _implement;

        private static IServiceProxy Implement
        {
            get
            {
                if (_implement == null)
                {
                    _implement = ServiceModelConfiguration.Current.Client.GetProxy() ?? _implementByRegister;
                    if (_implement == null) throw new ServiceException(Strings.NotFoundServiceProxy);
                }
                return _implement;
            }
        }


        private static IServiceProxy _implementByRegister;

        /// <summary>
        /// 注册服务代理，请确保<paramref name="implement"/>是线程访问安全的
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="repository"></param>
        public static void Register(IServiceProxy implement)
        {
            _implementByRegister = implement;
        }

        #endregion
    }
}
