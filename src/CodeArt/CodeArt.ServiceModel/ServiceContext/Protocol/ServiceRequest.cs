using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

using CodeArt.Util;
using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public struct ServiceRequest
    {
        /// <summary>
        /// 服务的名称空间
        /// </summary>
        public string Namespace
        {
            get;
            private set;
        }

        /// <summary>
        /// 服务的名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public string FullName
        {
            get
            {
                return _getFullName(this.Namespace)(this.Name);
            }
        }


        public DTObject Identity
        {
            get;
            private set;
        }

        public DTObject Argument
        {
            get;
            private set;
        }

        public int? TransmittedLength
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName">服务的名称，可以带命名空间，如果不带命名空间使用当前项目配置的默认命名空间</param>
        /// <param name="identity"></param>
        /// <param name="argument"></param>
        public ServiceRequest(string serviceName, DTObject identity, DTObject argument)
        {
            this.Namespace = _getNamespace(serviceName);
            this.Name = _getName(serviceName);
            this.Identity = identity ?? DTObject.Empty;
            this.Argument = argument ?? DTObject.Empty;
            this.TransmittedLength = null;
        }

        public ServiceRequest(string ns, string name, DTObject identity, DTObject argument)
        {
            this.Namespace = ns;
            this.Name = name;
            this.Identity = identity ?? DTObject.Empty;
            this.Argument = argument ?? DTObject.Empty;
            this.TransmittedLength = null;
        }

        #region 静态成员

        public static string GetName(string fullName)
        {
            return _getName(fullName);
        }

        private static Func<string, string> _getName = LazyIndexer.Init<string, string>((fullName) =>
        {
            var pos = fullName.IndexOf(":");
            if (pos == -1) return fullName;
            return fullName.Substring(pos + 1);
        });

        public static string GetNamespace(string fullName)
        {
            return _getNamespace(fullName);
        }

        private static Func<string, string> _getNamespace = LazyIndexer.Init<string, string>((fullName) =>
        {
            var pos = fullName.IndexOf(":");
            if (pos == -1) return ServiceModelConfiguration.Current.Client.DefaultServiceNamespace;
            return fullName.Substring(0, pos);
        });

        private static Func<string, Func<string, string>> _getFullName = LazyIndexer.Init<string, Func<string, string>>((ns) =>
        {
            return LazyIndexer.Init<string, string>((serviceName)=>
            {
                if (string.IsNullOrEmpty(ns)) return serviceName;
                return string.Format("{0}:{1}", ns, serviceName);
            });
        });

        /// <summary>
        /// 根据dto定义，得到ServiceRequest
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static ServiceRequest Create(DTObject dto)
        {
            var serviceName = dto.GetValue<string>("serviceName", string.Empty);
            var identity = dto.GetObject("identity", DTObject.Empty);
            var argument = dto.GetObject("argument", DTObject.Empty);
            var transmittedLength = dto.GetValue<int>("transmittedLength",-1);
            var request = new ServiceRequest(serviceName, identity, argument);
            if (transmittedLength > -1) request.TransmittedLength = transmittedLength;
            return request;
        }

        #endregion

    }
}
