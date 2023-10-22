using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DTO;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    public class DarkFilter : IAppDarkFilter
    {
        private DarkFilter()
        {
        }

        public bool InDark(DTObject arg)
        {
            long userId = arg.GetValue<long>("userId");

            // 判断用户是否灰度用户
            var data = ServiceContext.InvokeDynamic("darkerVerify", (g) =>
            {
                g.UserId = userId;
            });

            return data.GetValue<bool>("verify");
        }

        public static readonly DarkFilter Instance = new DarkFilter();
    }
}
