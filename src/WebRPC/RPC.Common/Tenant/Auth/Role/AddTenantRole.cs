using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Security;
using CodeArt.AppSetting;

namespace RPC.Common
{
    [Procedure("AddTenantRole")]
    [SafeAccess()]

    public class AddTenantRole : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addTenantRole", (g) =>
            {
                g.Name = arg.Name;
                g.PlatformId = arg.PlatformId;
                g.PermissionCodes = arg.PermissionCodes;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
                g.TenantId = AppSession.TenantId;
            });

            return data;
        }

    }
}


