using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace TestCoderSubsystem
{
    /// <summary>
    /// 创建动物信息的命令
    /// </summary>
    public class CreateAnimal : Command
    {
        private string _name;

        public string Description
        {
            get;
            set;
        }

        public CreateAnimal(string name)
        {
            _name = name;
        }


        protected override void ExecuteProcedure()
        {
            throw new NotImplementedException();
        }
    }
}
