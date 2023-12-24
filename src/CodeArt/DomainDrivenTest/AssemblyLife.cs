using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.AppSetting;

namespace DomainDrivenTest
{
    /// <summary>
    /// 由于AssemblyInitialize和AssemblyCleanup特性必须用于测试的程序集下，
    /// 封装进框架里会无效，所以我们只能在每个测试项目里使用该类来初始化和清理
    /// </summary>
    [TestClass]
    public class AssemblyLife
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            AppInitializer.Initialize();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            AppInitializer.Cleanup();
        }

    }
}
