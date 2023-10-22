using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using HtmlAgilityPack;
using CodeArt;
using CodeArt.Web;

namespace RPC.Common
{
    [SafeAccess]
    public class LogExtractor : ILogExtractor
    {
        public DTObject GetContent(IProcedure procedure, DTObject arg)
        {
            DTObject content = DTObject.Create();
            content["EventType"] = "rpc";
            content["Path"] = procedure.Path;
            content["IP"] = WebPageContext.Current.Request.UserHostAddress;

            long userId = arg.GetValue<long>("userId", 0);
            if(userId > 0)
            {
                content["UserId"] = userId;
            }
            return content;
        }
    }
}