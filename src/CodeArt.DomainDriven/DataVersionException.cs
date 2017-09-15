using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    public class DataVersionException : DomainDrivenException
    {
        public DataVersionException(Type objectType, object id)
            : base(string.Format(Strings.DataVersionError, objectType.FullName, id))
        {
        }
    }
}
