using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    internal sealed class ServiceAddressTable
    {
        /// <summary>
        /// 服务地址表的服务命名空间
        /// </summary>
        public string ServiceNamespace
        {
            get;
            private set;
        }

        private MultiDictionary<string, string> _data;


        public ServiceAddressTable(string serviceNamespace, MultiDictionary<string, string> data)
        {
            this.ServiceNamespace = serviceNamespace;
            _data = data;
        }

        /// <summary>
        /// 获得服务对应的地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public string GetAddress(string serviceName)
        {
            IList<string> addresses = null;
            if (_data.TryGetValue(serviceName, out addresses))
            {
                return Algorithm.Balance<string>(addresses); //如果有多个地址，那么均衡使用某一个地址
            }
            return null;
        }


    }
}
