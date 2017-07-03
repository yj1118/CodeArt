using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<T>
    {
        T Execute();
    }

}
