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
    [Procedure("AddPluginCategory")]
    [SafeAccess()]
    public class AddPluginCategory : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addPluginCategory", (g) =>
            {
                g.Name = arg.Name;
                g.Description = arg.Description;
            });

            return data;
        }


    }
}


