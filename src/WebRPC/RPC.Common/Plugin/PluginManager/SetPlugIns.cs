using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("SetPlugIns")]
    [SafeAccess()]
    public class SetPlugIns : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("setPlugIns", (g) =>
            {
                g.Id = arg.Id;
                g.PluginIds = arg.PluginIds;
            });

            return data;
        }
    }
}


