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
    [Procedure("GetPluginPage")]
    [SafeAccess()]
    public class GetPluginPage : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getPluginPage", (g) =>
            {
                g.Flag = arg.Flag;
                g.CategoryId = arg.CategoryId;
                g.TagId = arg.TagId;
                g.PageSize = arg.PageSize;
                g.PageIndex = arg.PageIndex;
            });

            return data;
        }
    }
}


