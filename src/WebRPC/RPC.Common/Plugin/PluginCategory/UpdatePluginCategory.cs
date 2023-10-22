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

namespace RPC.Common
{
    [Procedure("UpdatePluginCategory")]
    [SafeAccess()]
    public class UpdatePluginCategory : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updatePluginCategory", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.Description = arg.Description;
            });

            return data;
        }
    }
}


