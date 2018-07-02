using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CodeArt.ModuleNest
{
    public interface IModuleHandlerRouter
    {
        IModuleHandler<Q, S> CreateHandler<Q, S>(string handlerKey) where Q : class where S : class;

    }
}
