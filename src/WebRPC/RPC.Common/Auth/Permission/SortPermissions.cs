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
//using RPC.Common;

namespace RPC.Common
{
    [Procedure("SortPermissions")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class SortPermissions : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            arg.Transform("items.value=>id");

            var data = ServiceContext.InvokeDynamic("sortPermissions", (g) =>
            {
                g.Items = arg.Items;
            });

            return data;
        }

    }
}


