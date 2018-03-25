using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public sealed class ModuleChangedEventArgs
    {
        public RtpCommunicator Communicator
        {
            get;
            private set;
        }

        public RtpModule Module
        {
            get;
            private set;
        }

        public bool Installed
        {
            get;
            private set;
        }


        public ModuleChangedEventArgs(RtpCommunicator communicator, RtpModule module, bool installed)
        {
            this.Communicator = communicator;
            this.Module = module;
            this.Installed = installed;
        }

    }

    public delegate void ModuleChangedEventHandler(object sender, ModuleChangedEventArgs e);
}
