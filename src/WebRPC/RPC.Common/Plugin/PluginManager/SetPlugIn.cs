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
    [Procedure("SetPlugIn")]
    [SafeAccess()]
    public class SetPlugIn : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("setPlugIn", (g) =>
            {
                g.Id = arg.Id;
                g.PlugInId = arg.PlugInId;
                g.Enable = arg.Enable;
            });

            return data;
        }
    }
}


