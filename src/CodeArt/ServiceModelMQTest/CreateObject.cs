using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace MeetingService.Application
{
    [SafeAccess]
    [Service("CreateObject")]
    public class CreateObject : ServiceProvider
    {
        public override DTObject Invoke(DTObject arg)
        {
            var code = arg.GetCode();
            return DTObject.Create(code);
        }
    }
}
