using CodeArt.AppSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Security;

[assembly: PreApplicationStart(typeof(CodeArt.ServiceModel.PreApplicationStart), "Initialize")]

namespace CodeArt.ServiceModel
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
      
        }
    }
}
