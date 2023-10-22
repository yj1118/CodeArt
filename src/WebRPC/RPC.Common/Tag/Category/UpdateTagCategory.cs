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
    [Procedure("UpdateTagCategory")]
    [SafeAccess()]
    public class UpdateTagCategory : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateTagCategory", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
            });

            return data;
        }


    }
}


