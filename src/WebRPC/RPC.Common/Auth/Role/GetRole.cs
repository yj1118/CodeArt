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
    [Procedure("GetRole")]
    [SafeAccess()]
    public class GetRole : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getRole", (g) =>
            {
                g.Id = arg.Id;
                g.Slim = arg.Slim;
                g.Detail = arg.Detail;
            });

            var isPublic = data.GetValue<bool>("IsPublic");
            data.Transform("!isPublic");
            data.SetValue("IsPublic", isPublic ? 1 : 0);

            return data;
        }

    }
}


