using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    [Procedure("GetTagCategories")]
    [SafeAccess()]
    public class GetTagCategories : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getTagCategories", (g) =>
            {
                g.Name = arg.Name;
                g.PageIndex = arg.PageIndex;
                g.PageSize = arg.PageSize;
                g.All = arg.All;
            });

            return data;
        }
    }
}
