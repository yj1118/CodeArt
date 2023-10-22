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
    [Procedure("GetPermissions")]
    [SafeAccess()]
    public class GetPermissions : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getPermissions", (g) =>
            {
                g.Sort = arg.Sort;
            });

            if (arg.Sort != null)
            {
                data.Transform("rows.id=>value;rows.name=>text");
            }

            return data;
        }
    }
}


