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
    [Procedure("DeletePluginCategory")]
    [SafeAccess()]
    public class DeletePluginCategory : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deletePluginCategory", (g) =>
            {
                g.Id = arg.Id; 
            });

            return data;
        }

    }
}


