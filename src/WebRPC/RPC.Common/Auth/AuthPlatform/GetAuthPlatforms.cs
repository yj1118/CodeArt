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
    [Procedure("GetAuthPlatforms")]
    [SafeAccess()]
    public class GetAuthPlatforms : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAuthPlatforms", (g) =>
            {
                g.Sort = arg.Sort;
            });


            if (arg.Sort != null)
            {
                data.Transform("rows.id=>value;rows.name=>text");
            }

            return data;
        }
    }
}


