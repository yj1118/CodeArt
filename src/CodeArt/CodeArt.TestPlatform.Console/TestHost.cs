using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;

using CodeArt;
using CodeArt.DTO;
using CodeArt.Log;
using CodeArt.Concurrent;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.TestPlatform
{
    public static class TestHost
    {
        internal static bool IsEnabled
        {
            get;
            set;
        }

        public static void Start(bool autoClose)
        {
            IsEnabled = false;

            TestEvents.TestStart += OnTestStart;
            TestEvents.TestEnd += OnTestEnd;
            TestEvents.TestCaseStart += OnTestCaseStart;
            TestEvents.TestCaseEnd += OnTestCaseEnd;
            TestEvents.TestError += OnTestError;

            AppInitializer.Initialize();

            AppInitializer.Initialized();

            Execute();//执行测试

            IsEnabled = true;
            
            if(!autoClose)
            {
                Console.WriteLine("按任意键退出");
                Console.ReadLine();
            }

            AppInitializer.Cleanup();

            IsEnabled = false;
        }

        private static void OnTestError(object sender, TestEvents.TestErrorArgs arg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("发生错误：");
            var assert = arg.Error as AssertException;
            if (assert != null)
            {
                Console.WriteLine(assert.Message);
            }
            else
            {
                Console.WriteLine(arg.Error.GetCompleteInfo());
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void OnTestCaseEnd(object sender, TestEvents.TestCaseArgs arg)
        {
            if(arg.Context.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("测试通过，耗时 {0} 毫秒", arg.Context.ElapsedMilliseconds));
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("测试失败");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(string.Format("运行完毕 - {0}", arg.CaseName));
        }

        private static void OnTestCaseStart(object sender, TestEvents.TestCaseArgs arg)
        {
            string tip = string.Format("正在运行用例 - {0}", arg.CaseName);
            if (!string.IsNullOrEmpty(arg.Tip.Description))
            {
                tip += string.Format("({0})", arg.Tip.Description);
            }
            Console.WriteLine(tip);
        }

        private static void Execute()
        {
            var scenes = TestSceneAttribute.GetScenes();
            foreach(var scene in scenes)
            {
                scene.Start();
            }
        }

        private static void OnTestStart(object sender, TestEvents.TestSceneArgs arg)
        {
            string tip = string.Format("测试开始 - {0}", arg.Tip.Name);
            if (!string.IsNullOrEmpty(arg.Tip.Description))
            {
                tip += string.Format("({0})", arg.Tip.Description);
            }

            Console.WriteLine(tip);
        }

        private static void OnTestEnd(object sender, TestEvents.TestSceneArgs arg)
        {
            Console.WriteLine(string.Format("测试结束 - {0}", arg.Tip.Name));
        }

    }
}
