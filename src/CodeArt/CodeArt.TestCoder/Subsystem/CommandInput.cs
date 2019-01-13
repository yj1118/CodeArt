using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.TestCoder.Subsystem
{
    public class CommandInput
    {
        public CommandInputType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public CommandInput(CommandInputType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }
    }
}
