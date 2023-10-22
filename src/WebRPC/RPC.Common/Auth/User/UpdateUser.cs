using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("UpdateUser")]
    [SafeAccess()]

    public class UpdateUser : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("UpdateUser", (g) =>
            {
                g.Id = arg.Id ?? AppSession.PrincipalId;
                g.Nickname = arg.Nickname;
                g.RealName = arg.RealName;
                g.Sex = arg.Sex;
                g.Email = arg.Email;
                g.MobileNumber = arg.MobileNumber;
                g.Password = arg.Password;
            });

            return data;
        }
    }
}


