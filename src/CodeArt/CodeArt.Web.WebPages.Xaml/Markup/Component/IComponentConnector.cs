using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    public interface IComponentConnector
    {
        void Connect(string connectionName, object target);

        DependencyObject Find(string connectionName);

    }

}
