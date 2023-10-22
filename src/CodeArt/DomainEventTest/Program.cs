using CodeArt.AppSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainEventTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInitializer.Initialize();

            Test();

            Console.WriteLine("测试结束");

            Console.ReadLine();
            AppInitializer.Cleanup();
        }



        private static void Test()
        {
            long helperId = 2;    //祝愿人编号
            long wishUserId = 1; //许愿人编号
            ConsumeFreeGiftEvent.PiecesValue = 10; //每次免费赠送10个碎片

            var cmd = new AddWishPieceFree(helperId, wishUserId);
            var result = cmd.Execute();

            Util.AreEqual(10, result.Pieces);
            Util.AreEqual(20, result.Points);


            var helper = User.GetOrCreate(helperId);
            Util.AreEqual(10, helper.Pieces);
            Util.AreEqual(20, helper.Points);
            Util.AreEqual(0, helper.Gift);

            var user = User.GetOrCreate(wishUserId);
            Util.AreEqual(0, user.Pieces);
            Util.AreEqual(0, user.Points);
            Util.AreEqual(10, user.Gift);

        }


    }
}
