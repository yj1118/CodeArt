using System;
using System.IO;
using System.Text;

using CodeArt.Util;
using CodeArt.IO;
using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public static class DataAnalyzer
    {
        /// <summary>
        /// 因为调用的服务的名称是固定的数量，所以可以永久缓存
        /// </summary>
        private static Func<string, byte[]> _getServiceNameData = LazyIndexer.Init<string, byte[]>((serviceName) =>
        {
            return serviceName.GetBytes(Encoding.UTF8);
        });

        private static Func<string, byte[]> _getServiceNamespaceData = LazyIndexer.Init<string, byte[]>((serviceNamespace) =>
        {
            return serviceNamespace.GetBytes(Encoding.UTF8);
        });

        /// <summary>
        /// 获取数据，以后可以单独提取职责，编写用于提高性能的算法
        /// 以后也可以追加智能压缩模式，当字节数少量时不压缩，当字节数大量时压缩
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="identity"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static void SerializeRequest(ServiceRequest request, ByteArray target)
        {
            var namespaceData = _getServiceNameData(request.Namespace);
            var nameData = _getServiceNameData(request.Name);
            var identityData = Map(request.Identity);
            var argData = Map(request.Argument);

            if (ServiceModelConfiguration.Current.Client.EnabledOldVersion)
            {
                target.Write(nameData.Length);
                target.Write(nameData);
                target.Write(identityData.Length);
                target.Write(identityData);
                target.Write(argData.Length);
                target.Write(argData);
            }
            else
            {
                target.Write(namespaceData.Length);
                target.Write(namespaceData);
                target.Write(nameData.Length);
                target.Write(nameData);
                target.Write(identityData.Length);
                target.Write(identityData);
                target.Write(argData.Length);
                target.Write(argData);
            }
        }

        public static ServiceRequest DeserializeRequest(ByteArray source)
        {
            if (ServiceModelConfiguration.Current.Client.EnabledOldVersion)
            {
                //读取
                var nameDataLength = source.ReadInt32();
                var nameData = source.ReadBytes(nameDataLength);
                var identityDataLength = source.ReadInt32();
                var identityData = source.ReadBytes(identityDataLength);
                var argDataLength = source.ReadInt32();
                var argData = source.ReadBytes(argDataLength);

                var name = nameData.GetString(Encoding.UTF8);
                var identity = Map(identityData);
                var arg = Map(argData);

                return new ServiceRequest(string.Empty, name, identity, arg);
            }
            else
            {
                //读取
                var namespaceDataLength = source.ReadInt32();
                var namespaceData = source.ReadBytes(namespaceDataLength);
                var nameDataLength = source.ReadInt32();
                var nameData = source.ReadBytes(nameDataLength);
                var identityDataLength = source.ReadInt32();
                var identityData = source.ReadBytes(identityDataLength);
                var argDataLength = source.ReadInt32();
                var argData = source.ReadBytes(argDataLength);

                var ns = namespaceData.GetString(Encoding.UTF8);
                var name = nameData.GetString(Encoding.UTF8);
                var identity = Map(identityData);
                var arg = Map(argData);

                return new ServiceRequest(ns, name, identity, arg);
            }


        }

        public static void SerializeResponse(ServiceResponse response, ByteArray target)
        {
            var statusData = Map(response.Status);
            var returnValueData = Map(response.ReturnValue);

            target.Write(statusData.Length);
            target.Write(statusData);

            target.Write(returnValueData.Length);
            target.Write(returnValueData);
        }


        public static ServiceResponse DeserializeResponse(ByteArray source)
        {
            var statusDataLength = source.ReadInt32();
            var statusData = source.ReadBytes(statusDataLength);
            var returnValueDataLength = source.ReadInt32();
            var returnValueData = source.ReadBytes(returnValueDataLength);

            var status = Map(statusData);
            var returnValue = Map(returnValueData);

            return new ServiceResponse(status, returnValue,BinaryResponse.Empty);
        }

        private static byte[] Map(DTObject dto)
        {
            return dto == null || dto.IsEmpty() ? Array.Empty<byte>() : dto.ToData();
        }

        private static DTObject Map(byte[] data)
        {
            return data.Length == 0 ? DTObject.Empty : DTObject.Create(data);
        }
    }
}
