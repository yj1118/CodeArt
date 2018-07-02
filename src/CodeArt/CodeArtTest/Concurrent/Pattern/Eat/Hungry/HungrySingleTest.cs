using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

using CodeArt.Concurrent;
using CodeArt.TestTools;
using CodeArt.Concurrent.Pattern;

namespace CodeArtTest.Concurrent.Eat
{
    [TestClass]
    public class HungrySingleTest : HungryTest
    {
        protected override bool IsSingle
        {
            get
            {
                return true;
            }
        }

    }
}
