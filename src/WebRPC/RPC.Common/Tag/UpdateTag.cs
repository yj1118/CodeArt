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
    [Procedure("UpdateTag")]
    [SafeAccess()]
    public class UpdateTag : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateTag", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
                g.CategoryIds = arg.CategoryIds;
            });

            return data;
        }


    }
}


