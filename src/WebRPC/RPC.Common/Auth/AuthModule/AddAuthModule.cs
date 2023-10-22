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

namespace RPC.Common
{
    [Procedure("AddAuthModule")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class AddAuthModule : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addAuthModule", (g) =>
            {
                g.PlatformId = arg.PlatformId;
                g.BelongValue = arg.BelongValue;
                g.BelongLabel = arg.BelongLabel;
                g.Name = arg.Name;
                g.Description = arg.Description;
            });

            return data;
        }

    }
}


