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
    [Procedure("GetAuthScope")]
    [SafeAccess()]
    public class GetAuthScope : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAuthScope", (g) =>
            {
                g.Id = arg.Id;
            });


            return data;
        }

    }
}


