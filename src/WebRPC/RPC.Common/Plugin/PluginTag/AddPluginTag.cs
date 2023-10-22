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
    [Procedure("AddPluginTag")]
    [SafeAccess()]
    public class AddPluginTag : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addPluginTag", (g) =>
            {
                g.Name = arg.Name;
                g.Description = arg.Description;
            });

            return data;
        }


    }
}


