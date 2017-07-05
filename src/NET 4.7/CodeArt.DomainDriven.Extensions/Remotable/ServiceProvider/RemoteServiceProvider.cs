using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

using CodeArt.Runtime;
using CodeArt.Util;


namespace CodeArt.DomainDriven.Extensions
{
    /// <summary>
    /// 远程服务提供者的基类
    /// </summary>
    public abstract class RemoteServiceProvider : ServiceProvider
    {
        public override DTObject Invoke(ServiceRequest request)
        {
            CertifiedIdentity(request.Identity);
            return InvokeImpl(request.Argument);
        }

        protected abstract DTObject InvokeImpl(DTObject arg);

        private void CertifiedIdentity(DTObject identity)
        {
            if (!_config.Authentication) return; //不需要验证
            string identityName = identity.GetValue<string>("name", string.Empty);
            if (_success.ContainsKey(identityName)) return;
            var target = _config.Memberships.FirstOrDefault((m)=>
            {
                return m.Name.EqualsIgnoreCase(identityName);
            });
            if(target != null)
            {
                _success.TryAdd(identityName, true);
                return;
            }
            throw new DomainDrivenException(string.Format(Strings.IdentityFailed, identityName));
        }

        #region 辅助方法

        protected void UseDefines(DTObject arg, Action<AggregateRootDefine, object> action)
        {
            var typeName = arg.GetValue<string>("typeName");
            var defines = RemoteType.GetDefines(typeName);
            foreach (var define in defines)
            {
                var idProperty = DomainProperty.GetProperty(define.MetadataType, EntityObject.IdPropertyName);
                var id = DataUtil.ToValue(arg.GetValue(EntityObject.IdPropertyName), idProperty.PropertyType);
                action((AggregateRootDefine)define, id);
            }
        }

        #endregion


        private static ConcurrentDictionary<string, bool> _success = new ConcurrentDictionary<string, bool>();

        private static RemotableConfig _config;

        static RemoteServiceProvider()
        {
            _config = DomainDrivenConfiguration.Current.RemotableConfig;
        }
    }
}