using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    [Procedure("GetRoles")]
    [SafeAccess()]
    public class GetRoles : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getRoles", (g) =>
            {
                g.TenantId = 0;
                g.Admin = true;
                g.PlatformId = arg.PlatformId;
            });

            return data;
        }
    }
}


