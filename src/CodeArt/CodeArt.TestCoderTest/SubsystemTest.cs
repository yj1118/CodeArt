using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestCoder.Subsystem;

namespace CodeArt.TestCoderTest
{
    [TestClass]
    public class SubsystemTest
    {
#if (DEBUG)
        private const string SubsystemPath = @"D:\Workspace\Projects\CodeArt Framework\CodeArt\TestCoderSubsystem\bin\Debug\TestCoderSubsystem.dll";
#endif

#if (!DEBUG)
        private const string SubsystemPath = @"D:\Workspace\Projects\CodeArt Framework\CodeArt\TestCoderSubsystem\bin\Release\TestCoderSubsystem.dll";
#endif

        [TestMethod]
        public void Common()
        {
            var sa = new SubsystemAssembly(SubsystemPath);

            Assert.AreEqual(1, sa.Commands.Count);

            var command = sa.Commands[0];
            Assert.AreEqual("CreateAnimal", command.Name);
        }
    }
}
