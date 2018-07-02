using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.ServiceModel;

namespace SerialNumberService
{
    class Program
    {
        static void Main(string[] args)
        {
            MQServiceHost.Start();
        }
    }
}
