﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.ServiceModel;

namespace ServiceModelMQTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MQServiceHost.Start();
        }
    }
}
