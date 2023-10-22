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
    [Procedure("GetTags")]
    [SafeAccess()]
    public class GetTags : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getTags", (g) =>
            {
                g.CategoryId = arg.CategoryId;
                g.CategoryMarkedCode = arg.CategoryMarkedCode;
                g.Name = arg.Name;
                g.PageIndex = arg.PageIndex;
                g.PageSize = arg.PageSize;
            });

            return data;
        }
    }
}
