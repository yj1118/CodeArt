using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace RPC.Common
{
    [Procedure("GetAccount")]
    [SafeAccess()]
    public class GetAccount : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAccount", (g) =>
            {
                g.Id = arg.Id ?? AppSession.PrincipalId;
                g.Auth = arg.Auth; //指定为“用户授权”页面调用
                g.Info = arg.Info;
                g.Ignore = arg.Ignore;
                g.RoleMarkedCode = arg.RoleMarkedCode;


            });

            if (arg.Auth != null)
            {
                data.SetValue("IsEnabled", data.GetValue<bool>("status.isEnabled"));
                data.Transform("!status");

                var roles = data.Dynamic.Auth.Roles;
                data.Transform("!auth");
                data["Roles"] = roles;
                data.Transform("Roles.id=>value;Roles.name=>text");
            }

            return data;
        }

    }
}


