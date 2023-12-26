﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;

namespace CodeArt.TestTools
{
    public abstract class TestDocument : ITestDocument
    {
        public IReact React
        {
            get;
            private set;
        }
    }
}