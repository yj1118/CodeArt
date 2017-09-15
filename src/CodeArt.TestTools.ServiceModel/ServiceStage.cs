using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace CodeArt.TestTools
{
    public class ServiceStage : UnitTestStage
    {
        #region 调用服务

        protected DTObject Invoke(string serviceName, Action<DTObject> fillArg)
        {
            return LocalServiceUtil.Invoke(serviceName, fillArg);
        }

        protected dynamic DynamicInvoke(string serviceName, Action<dynamic> fillArg)
        {
            return LocalServiceUtil.Invoke(serviceName, fillArg);
        }

        #endregion

    }
}
