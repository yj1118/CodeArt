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

namespace RPC.Common
{
    [Procedure("AddTagCategory")]
    [SafeAccess()]
    public class AddTagCategory : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addTagCategory", (g) =>
            {
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
            });

            return data;
        }


    }
}


