using System;
using System.Linq;
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
        public void Simple()
        {
            var sa = new SubsystemAssembly(SubsystemPath);

            var cmd = sa.FindCommand("CreateAnimal");
            Assert.IsNotNull(cmd);
            Assert.AreEqual("CreateAnimal", cmd.Name);
            Assert.AreEqual(2, cmd.Inputs.Count());

            var nameInput = cmd.FindInput("name");
            Assert.IsNotNull(nameInput);
            Assert.AreEqual("name", nameInput.Name);
            Assert.AreEqual(CommandInputType.ConstructorParameter, nameInput.Type);

            var descriptionInput = cmd.FindInput("description");
            Assert.IsNotNull(descriptionInput);
            Assert.AreEqual("Description", descriptionInput.Name);
            Assert.AreEqual(CommandInputType.Property, descriptionInput.Type);

            Assert.IsNull(cmd.Output);
        }
    }
}
