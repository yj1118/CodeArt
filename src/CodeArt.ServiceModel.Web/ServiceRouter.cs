using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 目前是永久缓存地址表信息，以后可以升级为同步更新模式
    /// </summary>
    [SafeAccess]
    internal class ServiceRouter
    {
        private DirectTable _direct;

        private ServiceRouter()
        {
            _direct = WebServiceModelConfiguration.Current.Client.Direct;
        }

        /// <summary>
        /// 获取提供服务的远程地址
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetAddress(ServiceRequest request)
        {
            return GetAddressByDirect(request) ?? GetAddressByRoute(request);
        }


        private string GetAddressByDirect(ServiceRequest request)
        {
            return _direct.GetAddress(request.FullName);
        }

        private string GetAddressByRoute(ServiceRequest request)
        {
            var table = _addresses.GetOrCreate(request.Namespace, (ns) =>
            {
                return DownloadAddressTable(ns, request.Identity); //由于同一个网站的身份相同，所以可以永久缓存
            });
            return table.GetAddress(request.Name);
        }



        private static LazyIndexer<string, ServiceAddressTable> _addresses = new LazyIndexer<string, ServiceAddressTable>();

        /// <summary>
        /// 下载地址表
        /// </summary>
        /// <returns></returns>
        private ServiceAddressTable DownloadAddressTable(string serviceNamespace, DTObject identity)
        {
            MultiDictionary<string, string> table = new MultiDictionary<string, string>(false);

            var routerAddresses = WebServiceModelConfiguration.Current.Client.GetAddresses(serviceNamespace);
            var routerAddress = Algorithm.Balance<string>(routerAddresses);

            if (routerAddress == null) return new ServiceAddressTable(serviceNamespace, table);


            //根据请求端身份，获取服务地址表
            DTObject arg = DTObject.CreateReusable();
            arg.SetValue("namespace", serviceNamespace); //提交服务的命名空间
            var request = new ServiceRequest("getServiceAddressTable", identity, arg);
            var response = WebServiceProxy.Instance.Invoke(request, routerAddress);
            response.TryCatch();
            var result = response.ReturnValue;
            result.Each("table", (item) =>
            {
                var address = item.GetValue<string>("address");
                var services = item.GetList("services").ToArray<string>();

                foreach(var service in services)
                {
                    table.Add(service, address);
                }
            });
            return new ServiceAddressTable(serviceNamespace, table);
        }

        private object _syncObject = new object();

        public static readonly ServiceRouter Instance = new ServiceRouter();

    }
}
