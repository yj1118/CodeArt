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
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("GetTenantRoles")]
    [SafeAccess()]
    public class GetTenantRoles : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getTenantRoles", (g) =>
            {
                g.TenantId = AppSession.TenantId;
                g.PlatformId = arg.PlatformId;
            });

            return data;
        }
    }
}

