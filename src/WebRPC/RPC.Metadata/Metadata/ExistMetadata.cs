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
    [Procedure("ExistMetadata")]
    [SafeAccess()]
    public class ExistMetadata : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = DTObject.Create();
            bool exist = false;

            string[] markedCodes = arg.MarkedCodes.OfType<string>();

            foreach (var markedCode in markedCodes)
            {
                var item = ServiceContext.InvokeDynamic("existMetadata", (g) =>
                {
                    g.MarkedCode = markedCode;
                });
                exist = item.GetValue<bool>("Exist");
                if (!exist) break;
            }

            data.SetValue("Exist", exist);
            return data;
        }


    }
}


