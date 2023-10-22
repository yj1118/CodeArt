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
    [Procedure("GetPermissionContent")]
    [SafeAccess()]
    public class GetPermissionContent : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getPermissionContent", (g) =>
            {
                g.PlatformId = arg.PlatformId;
                g.BelongValues = arg.BelongValues;
            });

            return data;
        }
    }
}


