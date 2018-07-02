using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace SerialNumberServiceClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInitializer.Initialize();


            Console.WriteLine("调用addGenerator的服务");


            //XY20170807
            //创建规则对象
            var hardcode = DTObject.CreateReusable();
            hardcode["ruleType"] = "hardcode";
            hardcode["content"] = "XY";

            var dateCode = DTObject.CreateReusable();
            dateCode["ruleType"] = "dateCode";

            var rules = new DTObject[] { hardcode, dateCode };


            //以下是调用远程服务的代码
            var result = ServiceContext.InvokeDynamic("addGenerator", (arg) =>
            {
                arg.Name = "第一个测试用的";
                arg.MarkedCode = "first";
                arg.Rules = rules;
            });

            //Console.WriteLine("调用deleteGenerator的服务");
            //var permissionId = result.GetValue<Guid>("id");

            //ServiceContext.Invoke("deletePermission", (arg) =>
            //{
            //    arg.Id = permissionId;
            //});

            //Console.WriteLine("执行完毕，按任意键退出");

            AppInitializer.Cleanup();

            Console.ReadLine();

        }
    }
}
