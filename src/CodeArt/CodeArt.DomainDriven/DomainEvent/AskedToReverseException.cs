using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 被要求回逆
    /// </summary>
    public class AskedToReverseException : DomainDrivenException
    {
        public AskedToReverseException(string message)
            : base(message)
        {

        }
    }
}
