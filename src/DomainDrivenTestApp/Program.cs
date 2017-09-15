using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using CodeArt.ServiceModel;
using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.DomainDriven;

namespace DomainDrivenTestApp
{
    class Program
    {
      
        static void Main(string[] args)
        {
            MQServiceHost.Start(Initialize);
        }


        private static void Initialize()
        {
            Console.WriteLine("正在初始化数据");

            DataContext.Using(() =>
            {
                {
                    var cmd = new DomainModel.DeleteUser(1);
                    cmd.Execute();

                    cmd = new DomainModel.DeleteUser(2);
                    cmd.Execute();

                    cmd = new DomainModel.DeleteUser(3);
                    cmd.Execute();

                    cmd = new DomainModel.DeleteUser(10);
                    cmd.Execute();
                }

                {
                    var cmd = new DomainModel.CreateUser(3, "崽崽", 0, 0);
                    cmd.Execute();

                    cmd = new DomainModel.CreateUser(2, "新星", 0, 3);
                    cmd.Execute();

                    cmd = new DomainModel.CreateUser(1, "袁俊", 2, 3);
                    cmd.Execute();

                    cmd = new DomainModel.CreateUser(10, "袁俊2", 0, 0);
                    cmd.Execute();
                }

            });
        }




    }
}
