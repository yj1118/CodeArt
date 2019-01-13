using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

using CodeArt;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.Web.Mobile
{
    /// <summary>
    /// 基于特性的服务提供工厂
    /// </summary>
    [SafeAccess]
    public class AttributeProcedureFactory : IProcedureFactory
    {
        public IProcedure Create(string virtualPath)
        {
            return ProcedureAttribute.GetProcedure(virtualPath);
        }

        public static readonly IProcedureFactory Instance = new AttributeProcedureFactory();
    }
}
