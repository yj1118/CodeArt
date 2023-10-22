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
    [Procedure("UpdateAccount")]
    [SafeAccess()]

    public class UpdateAccount : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateAccount", (g) =>
            {
                g.Id = arg.Id ?? AppSession.PrincipalId;
                g.IgnoreAuth = arg.IgnoreAuth;
                g.IgnoreDark = arg.IgnoreDark;
                g.Test = arg.Test;

                g.Email = arg.Email;
                g.MobileNumber = arg.MobileNumber;
                g.Password = arg.Password;
                g.RoleIds = arg.RoleIds;
                if (arg.Exist("IsEnabled"))
                {
                    g.IsEnabled = arg.GetValue<int>("IsEnabled") == 1 ? true : false;
                }
            });

            return data;
        }
    }
}


