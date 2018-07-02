using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.ServiceModel;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("SendSMS")]
    public class SendSMS : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var provider = arg.Provider;
            if (string.IsNullOrEmpty(provider)) provider = "winic";

            IEnumerable<string> mobileNumbers = arg.MobileNumbers.OfType<string>();

            ISMSSender sender = SMSSenderProvider.GetSender(provider);
            if (sender == null) throw new BusinessException(string.Format("没有找到短信提供商{0}", provider));

            var message = arg.Message;

            Parallel.ForEach(mobileNumbers, (mobileNumber) =>
            {
                if (string.IsNullOrEmpty(mobileNumber)) return;

                sender.Send(mobileNumber, message);
            });
            return DTObject.Empty;
        }
    }

}
