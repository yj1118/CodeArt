using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IEventHandler
    {
        void Process(object sender, object e);
    }
}
