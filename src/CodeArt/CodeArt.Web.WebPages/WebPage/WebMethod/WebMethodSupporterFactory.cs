using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    public static class WebMethodSupporterFactory
    {
        /// <summary>
        /// 注册参数提取者
        /// </summary>
        /// <param name="router"></param>
        public static void RegisterExtractor(IParameterExtractor extractor)
        {
            SafeAccessAttribute.CheckUp(extractor.GetType());
            _extractor = extractor;
        }

        public static IParameterExtractor CreateExtractor()
        {
            if (_extractor == null) return DTOExtractor.Instance;
            return _extractor;
        }

        private static IParameterExtractor _extractor = null;


        /// <summary>
        /// 注册结果发送者
        /// </summary>
        /// <param name="router"></param>
        public static void RegisterSender(IResultSender sender)
        {
            SafeAccessAttribute.CheckUp(sender.GetType());
            _sender = sender;
        }

        public static IResultSender CreateSender()
        {
            if (_sender == null) return DTOSender.Instance;
            return _sender;
        }

        private static IResultSender _sender = null;


    }
}
