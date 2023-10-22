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
    [Procedure("GetAuthPlatform")]
    [SafeAccess()]
    public class GetAuthPlatform : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAuthPlatform", (g) =>
            {
                g.Id = arg.Id;
                g.EN = arg.EN;
            });


            return data;
        }

    }
}


