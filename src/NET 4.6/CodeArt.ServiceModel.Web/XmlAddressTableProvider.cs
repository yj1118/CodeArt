using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 基于xml服务地址表提供者
    /// </summary>
    [SafeAccess()]
    public class XmlAddressTableProvider : ServiceProvider
    {
        public override DTObject Invoke(DTObject arg)
        {
            var serviceNamespace = arg.GetValue<string>("namespace");
            return _getTable(serviceNamespace);
        }

        private static Func<string, DTObject> _getTable = LazyIndexer.Init<string, DTObject>((serviceNamespace)=>
        {
            return BuildTable(serviceNamespace);
        });


        private static MultiDictionary<string, string> Load(string fileName)
        {
            MultiDictionary<string, string> table = new MultiDictionary<string, string>(false);
            try
            {
                var nodes = GetNodes(fileName);
                foreach (XmlNode node in nodes)
                {
                    var address = node.GetAttributeValue("address");
                    if (string.IsNullOrEmpty(address)) continue;
                    var services = node.GetAttributeValue("services").Trim(";");
                    if (string.IsNullOrEmpty(services)) continue;

                    var temp = services.Split(';');
                    foreach(var service in temp)
                    {
                        if (string.IsNullOrEmpty(service)) continue;
                        table.Add(address, service);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return table;
        }

        private static DTObject BuildTable(string serviceNamespace)
        {
            if (string.IsNullOrEmpty(serviceNamespace))
                serviceNamespace = "default";
            var fileName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, string.Format("{0}.xml", serviceNamespace));
            var table = Load(fileName);

            var dto = DTObject.Create();
            foreach(var item in table)
            {
                var row = dto.CreateAndPush("table");
                var address = item.Key;
                row.SetValue("address", address);

                var services = item.Value;
                row.Push("services", services, (t, service) =>
                 {
                     t.SetValue(service);
                 });
            }
            return dto;
        }

        private static XmlNodeList GetNodes(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            return doc.SelectNodes("table/add");
        }


    }
}
