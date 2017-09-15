using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

using CodeArt.DTO;
using CodeArt.AppSetting;

namespace CodeArt.ServiceModel
{
    public static class ServiceHost
    {
        public static DTObject Identity
        {
            get
            {
                return AppSession.GetItem<DTObject>("ServiceClientIdentity");
            }
            set
            {
                AppSession.SetItem("ServiceClientIdentity", value);
            }
        }
    }
}
