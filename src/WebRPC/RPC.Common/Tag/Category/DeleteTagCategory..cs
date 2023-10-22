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
using CodeArt.AppSetting;
using RPC.Common;

namespace RPC.Main
{
    [Procedure("deleteTagCategory")]
    [SafeAccess()]
    public class DeleteTagCategory : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteTagCategory", (g) =>
            {
                g.Id = arg.Id;
            });

            return data;
        }


    }
}


