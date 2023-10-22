using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 代表命令的接口
    /// </summary>
    public interface ICommand
    {

    }

    public interface ICommandImp: ICommand
    {
        void Execute();
    }

    public interface ICommandImp<T> : ICommand
    {
        T Execute();
    }

}
