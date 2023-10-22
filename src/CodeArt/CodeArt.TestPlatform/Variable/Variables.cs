using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.TestPlatform
{
    public static class Variables
    {
        /// <summary>
        /// 查找默认命名空间下的变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DTObject Get(string name)
        {
            return Get(string.Empty, name);
        }

        public static DTObject Get(string package, string name)
        {
            var provider = VariableProviderFactory.Create(package, name);
            return provider.Get(package, name);
        }
    }
}
