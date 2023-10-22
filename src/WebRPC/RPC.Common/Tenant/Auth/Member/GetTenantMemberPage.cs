using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("GetTenantMemberPage")]
    [SafeAccess()]

    public class GetTenantMemberPage : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("GetTenantMemberPage", (g) =>
            {
                g.Name = arg.Name;
                g.ToUser = arg.ToUser;
                g.TenantId = AppSession.TenantId;
                g.PageIndex = arg.PageIndex;
                g.PageSize = arg.PageSize;
            });

            return data;
        }

    }
}


