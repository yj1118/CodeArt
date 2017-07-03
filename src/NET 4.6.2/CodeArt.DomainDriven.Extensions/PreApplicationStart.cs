using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.ServiceModel;

[assembly: PreApplicationStart(typeof(CodeArt.DomainDriven.Extensions.PreApplicationStart), "Initialize", PreApplicationStartPriority.Height)]

namespace CodeArt.DomainDriven.Extensions
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            //注册领域模型层中关于远程服务的实现类
            Repository.Register(RemoteServiceImpl.Instance);

            //令服务提供者为远程服务的提供者工厂
            //这样可以自动处理远程对象的事务而无需程序员操心
            //如果请求不是远程对象相关的服务，那么使用常规服务处理请求
            ServiceProviderFactory.Register(RemoteServiceProviderFactory.Instance);
        }
    }
}
