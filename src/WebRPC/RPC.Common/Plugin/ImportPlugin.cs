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

namespace RPC.Common
{
    [Procedure("ImportPlugin")]
    [SafeAccess()]
    public class ImportPlugin : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {

            var data = ServiceContext.InvokeDynamic("importPlugin", (g) =>
            {
                g.Local = arg.Local;
            });

            return data;
        }


    }
}


