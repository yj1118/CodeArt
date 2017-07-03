using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    [NonLog]
    public class IsNullException : DomainDrivenException
    {
        public IsNullException()
            : base()
        {
        }

        public IsNullException(string source)
            : base(string.Format(Strings.IsEmpty, source))
        {
        }
    }
}
