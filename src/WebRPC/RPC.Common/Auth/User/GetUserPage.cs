using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Util;

namespace RPC.Common
{
    [Procedure("GetUserPage")]
    [SafeAccess()]
    public class GetUserPage : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAllUserPage", (g) =>
            {
                g.Name = arg.Name;
                g.Auth = true; //指定为“用户授权”页面调用
                g.PageSize = arg.PageSize;
                g.PageIndex = arg.PageIndex;
            });

            data.Each("rows", (row) =>
            {
                HandleData(row);
            });

            return data;
        }

        private void HandleData(DTObject row)
        {
            if (row.Exist("Account.Status.LoginInfo"))
            {
                var lastTime = row.GetValue<DateTime>("Account.Status.LoginInfo.LastTime");
                var total = row.GetValue<string>("Account.Status.LoginInfo.Total");
                row.Transform("!account.status.loginInfo");
                row.SetValue("LastTime", lastTime.Humanize());
                row.SetValue("Total", total);
            }
            else
            {
                row.SetValue("LastTime", string.Empty);
                row.SetValue("Total", string.Empty);
            }
        }
    }
}


