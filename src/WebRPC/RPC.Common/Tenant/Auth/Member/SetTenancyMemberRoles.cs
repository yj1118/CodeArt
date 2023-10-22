using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("SetTenancyMemberRoles")]
    [SafeAccess()]
    public class SetTenancyMemberRoles : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("setTenantMemberRoles", (g) =>
            {
                g.AccountId = arg.AccountId;
                g.RoleIds = arg.RoleIds;
                g.TenantId = AppSession.TenantId;
            });

            return data;
        }
    }
}


