using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    public class SnapshotTargetException : DomainDrivenException
    {
        public SnapshotTargetException()
            : base(Strings.SnapshotTargetError)
        {
        }

        public SnapshotTargetException(string message)
            : base(message)
        {
        }
    }
}
