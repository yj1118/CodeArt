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
    [Procedure("AddTag")]
    [SafeAccess()]
    public class AddTag : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addTag", (g) =>
            {
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
                g.CategoryIds = arg.CategoryIds;
            });

            return data;
        }


    }
}


