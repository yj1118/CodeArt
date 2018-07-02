using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace ProtalServiceClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInitializer.Initialize();


            Console.WriteLine("调用addPermission的服务");

            //以下是调用远程服务的代码
            var result = ServiceContext.Invoke("AddPermission", (arg) =>
             {
                 //arg.Name = "测试权限的名称";
             });

            Console.WriteLine("调用deletePermission的服务");
            var permissionId = result.GetValue<Guid>("id");

            ServiceContext.Invoke("deletePermission", (arg) =>
            {
                //arg.Id = permissionId;
            });

            Console.WriteLine("执行完毕，按任意键退出");

            AppInitializer.Cleanup();

            Console.ReadLine();

        }
    }
}
