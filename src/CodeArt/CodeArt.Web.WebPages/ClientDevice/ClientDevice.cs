using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Hosting;

namespace CodeArt.Web.WebPages
{
    public sealed class ClientDevice
    {

        public bool IsMobile
        {
            get;
            set;
        }

        public ClientDevice()
        {
            this.IsMobile = false;
        }
    }
}
