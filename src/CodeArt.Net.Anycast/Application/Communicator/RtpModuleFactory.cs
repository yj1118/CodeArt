using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    public abstract class RtpModuleFactory : IRtpModuleFactory
    {
        public IEnumerable<RtpModule> Create()
        {
            var single = CreateSingle();
            var multiple = CreateMultiple();
            if (single == null && multiple == null) return null;

            List<RtpModule> modules = new List<RtpModule>();
            if (single != null) modules.Add(single);
            if (multiple != null) modules.AddRange(multiple);
            return modules;
        }

        protected virtual RtpModule CreateSingle()
        {
            return null;
        }

        protected virtual IEnumerable<RtpModule> CreateMultiple()
        {
            return null;
        }

    }
}