using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.TestCoder.Subsystem
{
    /// <summary>
    /// 子系统程序集
    /// </summary>
    public class SubsystemAssembly
    {
        public Assembly Assembly
        {
            get;
            private set;
        }

        public SubsystemAssembly(string assemblyFileName)
        {
            this.Assembly = Assembly.LoadFrom(assemblyFileName);
            this.Commands = GetCommands();
        }

        #region 子系统提供的命令

        public CommandInfo FindCommand(string commandName)
        {
            return this.Commands.FirstOrDefault((c) => c.Name.EqualsIgnoreCase(commandName));
        }


        public IEnumerable<CommandInfo> Commands
        {
            get;
            private set;
        }

        private IEnumerable<CommandInfo> GetCommands()
        {
            var commandTypes = this.Assembly.GetImplementTypes(typeof(ICommand));
            List<CommandInfo> commands = new List<CommandInfo>(commandTypes.Count());
            foreach (var commandType in commandTypes)
            {
                var name = commandType.Name;
                var isEvent = commandType.IsImplementOrEquals(typeof(IEventCommand));
                var inputs = GetCommandInputs(commandType);

                var command = new CommandInfo(name, isEvent, inputs, null);
                commands.Add(command);
            }

            return commands;
        }

        private static IEnumerable<CommandInput> GetCommandInputs(Type commandType)
        {
            List<CommandInput> inputs = new List<CommandInput>();
            var constructors = commandType.GetConstructors();

            if (constructors.Count() == 0)
            {
                throw new ApplicationException(string.Format("子系统命令{0}无效，没有构造函数", commandType.Name));
            }
            else if (constructors.Count() == 1)
            {
                var constructor = constructors.First();
                //根据参数获取输入
                var prms = constructor.GetParameters();
                foreach(var prm in prms)
                {
                    var input = new CommandInput(CommandInputType.ConstructorParameter, prm.Name);
                    inputs.Add(input);
                }
            }
            else
            {
                throw new ApplicationException(string.Format("子系统命令{0}无效,当前版本不支持子系统命令有多个构造函数的情况", commandType.Name));
            }

            //根据可以set的属性，获取输入
            var propertyInfos = commandType.GetProperties(_propertyFlags);
            foreach(var propertyInfo in propertyInfos)
            {
                if(propertyInfo.IsPublicSet())
                {
                    var input = new CommandInput(CommandInputType.Property, propertyInfo.Name);
                    inputs.Add(input);
                }
            }
            return inputs;
        }

        private const BindingFlags _propertyFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;


        #endregion
    }
}
