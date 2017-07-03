using System;

namespace CodeArt.ModuleNest
{
    public class ModuleException : Exception
    {
        public ModuleException()
            : base()
        {
        }

        public ModuleException(string message)
            : base(message)
        {
        }
    }
}
