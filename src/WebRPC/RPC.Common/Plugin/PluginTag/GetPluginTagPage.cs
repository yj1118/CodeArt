using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

using RPC.Main;

namespace RPC.Common
{
    [Procedure("GetPluginTagPage")]
    [SafeAccess()]
    public class GetPluginTagPage : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getPluginTagPage", (g) =>
            {
                g.Name = arg.Name;
                g.PageSize = arg.PageSize;
                g.PageIndex = arg.PageIndex;
            });

            return data;
        }
    }
}


