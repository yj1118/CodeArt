using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

using SerialNumberSubsystem;

namespace SerialNumberService
{
    /// <summary>
    /// 新增一个流水号生成器
    /// </summary>
    [SafeAccess]
    [Service("addGenerator")]
    public class AddGenerator : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new CreateGenerator(arg.Name, arg.Rules)
            {
                MarkedCode = arg.MarkedCode
            };
            var g = cmd.Execute();
            return DTObject.CreateReusable("{id}", g);
        }
    }
}
