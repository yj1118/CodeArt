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

namespace RPC.Metadata
{
    [Procedure("HasMetadatas")]
    [SafeAccess()]
    public class HasMetadatas : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = DTObject.Create();

            ((DTObject)arg).Each("MarkedCodes", (t) =>
            {
                var code = t.GetValue<string>();
                var item = ServiceContext.InvokeDynamic("hasMetadata", (g) =>
                {
                    g.MarkedCode = code;
                });

                data.SetValue(code, item.GetValue<bool>("Has"));
            });

            return data;
        }
    }
}


