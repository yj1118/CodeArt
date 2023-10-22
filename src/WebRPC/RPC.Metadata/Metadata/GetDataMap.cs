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

namespace RPC.Metadata
{
    [Procedure("GetDataMap")]
    [SafeAccess()]
    public class GetDataMap : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getDataMap", (g) =>
            {
                g.Context = arg.Context;
                g.Precise = arg.Precise;
            });

            return data;
        }


    }
}


