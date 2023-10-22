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
    [Procedure("UpdatePasswordByMobileNumber", typeof(LogExtractor))]
    [SafeAccess()]
    public class UpdatePasswordByMobileNumber : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var result = ServiceContext.InvokeDynamic("updatePasswordByMobileNumber", (g) =>
            {
                g.MobileNumber = arg.MobileNumber;
                g.Code = arg.Code;
                g.NewPassword = arg.NewPassword;
            });

            return result;
        }
    }
}