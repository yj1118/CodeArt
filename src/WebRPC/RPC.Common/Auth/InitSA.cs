using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using RPC.Common;
using CodeArt.ServiceModel;
using CodeArt;

namespace RPC.Common
{
    [Procedure("InitSA", typeof(LogExtractor))]
    [SafeAccess()]
    public class InitSA : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var result = ServiceContext.InvokeDynamic("initSA", (g) =>
            {
                g.AccountName = arg.AccountName;
                g.Password = arg.Password;
            });

            return result;
        }
    }
}