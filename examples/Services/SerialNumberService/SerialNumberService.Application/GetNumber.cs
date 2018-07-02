using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using SerialNumberSubsystem;
using CodeArt.DomainDriven;

namespace SerialNumberService.Application
{
    [SafeAccess]
    [Service("GetNumber")]
    public class GetNumber : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            SNGenerator generator = SNGeneratorCommon.FindByMarkedCode(arg.MarkedCode, QueryLevel.None);
            var number = generator.Generate();
            var result = DTObject.CreateReusable();
            result["number"] = number;
            return result;
        }
    }
}
