using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.TestCoder.Subsystem
{
    /// <summary>
    /// 子系统程序集
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// 命令的名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 该命令是否涉及到领域事件，涉及到领域事件的命令会有分布式应用，因此该命令所处的测试不会是小测试
        /// </summary>
        public bool IsEvent
        {
            get;
            private set;
        }

        /// <summary>
        /// 命令接受的输入（构造函数的参数和对外公开可以设置的属性）
        /// </summary>
        public IEnumerable<CommandInput> Inputs
        {
            get;
            private set;
        }


        public CommandInput FindInput(string inputName)
        {
            return this.Inputs.FirstOrDefault((c) => c.Name.EqualsIgnoreCase(inputName));
        }

        /// <summary>
        /// 返回值
        /// </summary>
        public CommandOutput Output
        {
            get;
            private set;
        }


        public CommandInfo(string name, bool isEvent, IEnumerable<CommandInput> inputs, CommandOutput output)
        {
            this.Name = name;
            this.IsEvent = isEvent;
            this.Inputs = inputs;
            this.Output = output;
        }


    }
}
