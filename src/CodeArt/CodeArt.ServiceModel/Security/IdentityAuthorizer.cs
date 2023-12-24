using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Security;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    public class IdentityAuthorizer : IAuthorizer<IdentityAttribute>
    {
        private IdentityAuthorizer() { }

        public bool Verify(IdentityAttribute attr, DTObject arg)
        {
            DTObject permission = string.IsNullOrEmpty(attr.Permission) ? null : _getPermission(attr.Permission);

            var result = ServiceContext.Invoke("auth:Verify", (g) =>
            {
                if (permission != null)
                    g["Permission"] = permission.GetValue<string>("code");

                if (!string.IsNullOrEmpty(attr.Role))
                    g["Role"] = attr.Role;

                g["target"] = arg.GetObject("target");  //被验证的目标
            });

            bool success = result.Exist("success");

            if (success)
            {
                //进一步判断
                if (permission != null)
                {
                    var scope = permission.GetValue<string>("scope", string.Empty);
                    if (!string.IsNullOrEmpty(scope))
                    {
                        var filter = AuthFilterFactory.Create();
                        if (!filter.Ignore(scope, arg))
                        {
                            //没有忽略此范围的验证
                            success = VerifyScope(attr, scope, arg);
                        }
                    }
                }
            }

            return success;
        }


        private static Func<string, string> _getVerifyScopeServiceName = LazyIndexer.Init<string, string>((resource)=>
        {
            return string.Format("auth:{0}", resource);
        });


        private bool VerifyScope(IdentityAttribute attr, string scope, DTObject arg)
        {
            if (string.IsNullOrEmpty(attr.Resource)) return true;// 没有指定资源，说明是不需要验证的

            var serviceName = _getVerifyScopeServiceName(attr.Resource);
            arg["scope"] = scope;
            var result = ServiceContext.Invoke(serviceName, arg);

            return result.Exist("success");
        }


        private static Func<string, DTObject> _getPermission = LazyIndexer.Init<string, DTObject>((code)=>
        {
            return DTObject.Create(code);
        });

        public static readonly IdentityAuthorizer Instance = new IdentityAuthorizer();

    }
}
