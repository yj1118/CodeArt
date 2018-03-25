using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 由于硬件故障引起的错误
    /// </summary>
    public class HardwareFailureException : DomainDrivenException
    {
        public HardwareFailureException(string message)
            : base(message)
        {

        }
    }
}
